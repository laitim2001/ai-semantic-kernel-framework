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
        var stats = (Total: 100, Successful: 90, Failed: 10, Cancelled: 0, AvgResponseTimeMs: 1500.5);

        _repositoryMock
            .Setup(x => x.GetStatisticsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var detailedMetrics = (
            MinResponseTimeMs: (double?)500.0,
            MaxResponseTimeMs: (double?)3000.0,
            MedianResponseTimeMs: (double?)1500.0,
            P95ResponseTimeMs: (double?)2500.0,
            P99ResponseTimeMs: (double?)2800.0,
            TotalTokensUsed: 50000L,
            AvgTokensPerExecution: 500.0,
            MinTokensUsed: (int?)100,
            MaxTokensUsed: (int?)1000
        );

        _repositoryMock
            .Setup(x => x.GetDetailedMetricsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(detailedMetrics);

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
        result.CancelledExecutions.Should().Be(0);
        result.SuccessRate.Should().Be(90.0);
        result.AverageResponseTimeMs.Should().Be(1500.5);
        result.MinResponseTimeMs.Should().Be(500.0);
        result.MaxResponseTimeMs.Should().Be(3000.0);
        result.MedianResponseTimeMs.Should().Be(1500.0);
        result.P95ResponseTimeMs.Should().Be(2500.0);
        result.P99ResponseTimeMs.Should().Be(2800.0);
        result.TotalTokensUsed.Should().Be(50000L);
        result.AverageTokensPerExecution.Should().Be(500.0);
        result.MinTokensUsed.Should().Be(100);
        result.MaxTokensUsed.Should().Be(1000);
    }

    [Fact]
    public async Task Handle_WithNoExecutions_ShouldReturnZeroSuccessRate()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var stats = (Total: 0, Successful: 0, Failed: 0, Cancelled: 0, AvgResponseTimeMs: 0.0);

        _repositoryMock
            .Setup(x => x.GetStatisticsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var detailedMetrics = (
            MinResponseTimeMs: (double?)null,
            MaxResponseTimeMs: (double?)null,
            MedianResponseTimeMs: (double?)null,
            P95ResponseTimeMs: (double?)null,
            P99ResponseTimeMs: (double?)null,
            TotalTokensUsed: 0L,
            AvgTokensPerExecution: 0.0,
            MinTokensUsed: (int?)null,
            MaxTokensUsed: (int?)null
        );

        _repositoryMock
            .Setup(x => x.GetDetailedMetricsAsync(
                agentId,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(detailedMetrics);

        var query = new GetAgentExecutionStatistics
        {
            AgentId = agentId
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalExecutions.Should().Be(0);
        result.SuccessfulExecutions.Should().Be(0);
        result.FailedExecutions.Should().Be(0);
        result.CancelledExecutions.Should().Be(0);
        result.SuccessRate.Should().Be(0);
        result.AverageResponseTimeMs.Should().Be(0);
        result.MinResponseTimeMs.Should().BeNull();
        result.MaxResponseTimeMs.Should().BeNull();
        result.MedianResponseTimeMs.Should().BeNull();
        result.P95ResponseTimeMs.Should().BeNull();
        result.P99ResponseTimeMs.Should().BeNull();
        result.TotalTokensUsed.Should().Be(0L);
        result.AverageTokensPerExecution.Should().Be(0.0);
        result.MinTokensUsed.Should().BeNull();
        result.MaxTokensUsed.Should().BeNull();
    }
}
