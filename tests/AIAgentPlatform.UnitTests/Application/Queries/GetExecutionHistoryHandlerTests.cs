using AIAgentPlatform.Application.AgentExecutions.Queries;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Queries;

public sealed class GetExecutionHistoryHandlerTests
{
    private readonly Mock<IAgentExecutionRepository> _repositoryMock;
    private readonly GetExecutionHistoryHandler _handler;

    public GetExecutionHistoryHandlerTests()
    {
        _repositoryMock = new Mock<IAgentExecutionRepository>();
        _handler = new GetExecutionHistoryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnExecutionHistory()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var execution1 = AgentExecution.Create(agentId, conversationId);
        execution1.MarkAsCompleted(100);

        var execution2 = AgentExecution.Create(agentId, conversationId);
        execution2.MarkAsCompleted(200);

        var executions = new List<AgentExecution> { execution1, execution2 };

        _repositoryMock
            .Setup(x => x.GetByAgentIdAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(executions);

        var query = new GetExecutionHistory
        {
            AgentId = agentId,
            Skip = 0,
            Take = 50
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].AgentId.Should().Be(agentId);
        result[0].Status.Should().Be("completed");
        result[0].TokensUsed.Should().Be(100);
        result[1].TokensUsed.Should().Be(200);
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        _repositoryMock
            .Setup(x => x.GetByAgentIdAsync(
                agentId,
                startDate,
                endDate,
                "completed",
                10,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AgentExecution>());

        var query = new GetExecutionHistory
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate,
            Status = "completed",
            Skip = 10,
            Take = 20
        };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.GetByAgentIdAsync(
                agentId,
                startDate,
                endDate,
                "completed",
                10,
                20,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
