using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class AgentStatusCommandTests
{
    private readonly Mock<IAgentRepository> _mockRepository;

    public AgentStatusCommandTests()
    {
        _mockRepository = new Mock<IAgentRepository>();
    }

    [Fact]
    public async Task ActivateAgent_WithValidId_ShouldActivateAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            Guid.NewGuid(),
            "Test Agent",
            "Test instructions",
            LLMModel.GPT4o
        );
        agent.Pause(); // Start with paused status

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var handler = new ActivateAgentCommandHandler(_mockRepository.Object);
        var command = new ActivateAgentCommand { Id = agentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("active");
        _mockRepository.Verify(x => x.UpdateAsync(agent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateAgent_WithNonExistentId_ShouldThrowAgentNotFoundException()
    {
        // Arrange
        var agentId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var handler = new ActivateAgentCommandHandler(_mockRepository.Object);
        var command = new ActivateAgentCommand { Id = agentId };

        // Act & Assert
        await Assert.ThrowsAsync<AgentNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task PauseAgent_WithValidId_ShouldPauseAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            Guid.NewGuid(),
            "Test Agent",
            "Test instructions",
            LLMModel.GPT4o
        );
        // Agent is active by default

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var handler = new PauseAgentCommandHandler(_mockRepository.Object);
        var command = new PauseAgentCommand { Id = agentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("paused");
        _mockRepository.Verify(x => x.UpdateAsync(agent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PauseAgent_WithNonExistentId_ShouldThrowAgentNotFoundException()
    {
        // Arrange
        var agentId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var handler = new PauseAgentCommandHandler(_mockRepository.Object);
        var command = new PauseAgentCommand { Id = agentId };

        // Act & Assert
        await Assert.ThrowsAsync<AgentNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ArchiveAgent_WithValidId_ShouldArchiveAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            Guid.NewGuid(),
            "Test Agent",
            "Test instructions",
            LLMModel.GPT4o
        );
        // Agent is active by default

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var handler = new ArchiveAgentCommandHandler(_mockRepository.Object);
        var command = new ArchiveAgentCommand { Id = agentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("archived");
        _mockRepository.Verify(x => x.UpdateAsync(agent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArchiveAgent_WithNonExistentId_ShouldThrowAgentNotFoundException()
    {
        // Arrange
        var agentId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var handler = new ArchiveAgentCommandHandler(_mockRepository.Object);
        var command = new ArchiveAgentCommand { Id = agentId };

        // Act & Assert
        await Assert.ThrowsAsync<AgentNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ActivateAgent_WhenAlreadyActive_ShouldRemainActive()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            Guid.NewGuid(),
            "Test Agent",
            "Test instructions",
            LLMModel.GPT4o
        );
        // Agent is active by default

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var handler = new ActivateAgentCommandHandler(_mockRepository.Object);
        var command = new ActivateAgentCommand { Id = agentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("active");
        _mockRepository.Verify(x => x.UpdateAsync(agent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PauseAgent_WhenAlreadyPaused_ShouldRemainPaused()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = Agent.Create(
            Guid.NewGuid(),
            "Test Agent",
            "Test instructions",
            LLMModel.GPT4o
        );
        agent.Pause();

        _mockRepository.Setup(x => x.GetByIdAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);

        var handler = new PauseAgentCommandHandler(_mockRepository.Object);
        var command = new PauseAgentCommand { Id = agentId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("paused");
        _mockRepository.Verify(x => x.UpdateAsync(agent, It.IsAny<CancellationToken>()), Times.Once);
    }
}
