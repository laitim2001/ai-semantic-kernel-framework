using AIAgentPlatform.Application.AgentExecutions.Commands;
using AIAgentPlatform.Application.AgentExecutions.Services;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class ExecuteAgentCommandHandlerTests
{
    private readonly Mock<IAgentRepository> _agentRepositoryMock;
    private readonly Mock<IAgentExecutionService> _executionServiceMock;
    private readonly ExecuteAgentCommandHandler _handler;

    public ExecuteAgentCommandHandlerTests()
    {
        _agentRepositoryMock = new Mock<IAgentRepository>();
        _executionServiceMock = new Mock<IAgentExecutionService>();
        _handler = new ExecuteAgentCommandHandler(
            _agentRepositoryMock.Object,
            _executionServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldExecuteSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var agent = Agent.Create(
            userId,
            "Test Agent",
            "You are a helpful assistant",
            LLMModel.GPT4o,
            0.7m,
            4096
        );

        var execution = AgentExecution.Create(agentId, conversationId);
        execution.MarkAsCompleted(tokensUsed: 100);

        _agentRepositoryMock
            .Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        _executionServiceMock
            .Setup(x => x.ExecuteAsync(
                It.IsAny<Agent>(),
                conversationId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((execution, "Test output"));

        var command = new ExecuteAgentCommand
        {
            AgentId = agentId,
            ConversationId = conversationId,
            Input = "Hello, world!"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("completed");
        result.Output.Should().Be("Test output");
        result.TokensUsed.Should().Be(100);

        _agentRepositoryMock.Verify(
            x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _executionServiceMock.Verify(
            x => x.ExecuteAsync(
                agent,
                conversationId,
                "Hello, world!",
                null,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNonExistentAgent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        _agentRepositoryMock
            .Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new ExecuteAgentCommand
        {
            AgentId = agentId,
            ConversationId = Guid.NewGuid(),
            Input = "Hello"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Agent {agentId} not found");
    }

    [Fact]
    public async Task Handle_WithInactiveAgent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            userId,
            "Test Agent",
            "Instructions",
            LLMModel.GPT4o,
            0.7m,
            4096
        );

        agent.Pause();

        _agentRepositoryMock
            .Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var command = new ExecuteAgentCommand
        {
            AgentId = agentId,
            ConversationId = Guid.NewGuid(),
            Input = "Hello"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Agent {agentId} is not active*");
    }
}
