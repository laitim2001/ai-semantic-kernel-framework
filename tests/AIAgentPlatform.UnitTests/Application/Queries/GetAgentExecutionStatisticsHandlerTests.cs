using AIAgentPlatform.Application.AgentExecutions.Queries;
using AIAgentPlatform.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Queries;

public sealed class GetAgentExecutionStatisticsHandlerTests
{
    private readonly Mock<IAgentExecutionRepository> _repositoryMock;
    private readonly GetAgentExecutionStatisticsHandler _handler;

    public GetAgentExecutionStatisticsHandlerTests()
    {
        _repositoryMock = new Mock<IAgentExecutionRepository>();
        _handler = new GetAgentExecutionStatisticsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnStatistics()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var stats = (Total: 100, Successful: 90, Failed: 10, AvgResponseTimeMs: 1500.5);

        _repositoryMock
            .Setup(x => x.GetStatisticsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var query = new GetAgentExecutionStatistics
        {
            AgentId = agentId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AgentId.Should().Be(agentId);
        result.TotalExecutions.Should().Be(100);
        result.SuccessfulExecutions.Should().Be(90);
        result.FailedExecutions.Should().Be(10);
        result.SuccessRate.Should().Be(90.0);
        result.AverageResponseTimeMs.Should().Be(1500.5);
    }

    [Fact]
    public async Task Handle_WithNoExecutions_ShouldReturnZeroSuccessRate()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var stats = (Total: 0, Successful: 0, Failed: 0, AvgResponseTimeMs: 0.0);

        _repositoryMock
            .Setup(x => x.GetStatisticsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var query = new GetAgentExecutionStatistics
        {
            AgentId = agentId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.SuccessRate.Should().Be(0);
    }
}
