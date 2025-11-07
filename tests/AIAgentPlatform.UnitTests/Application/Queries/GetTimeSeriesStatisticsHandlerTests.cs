using AIAgentPlatform.Application.AgentExecutions.Queries;
using AIAgentPlatform.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Queries;

public sealed class GetTimeSeriesStatisticsHandlerTests
{
    private readonly Mock<IAgentExecutionRepository> _repositoryMock;
    private readonly GetTimeSeriesStatisticsHandler _handler;

    public GetTimeSeriesStatisticsHandlerTests()
    {
        _repositoryMock = new Mock<IAgentExecutionRepository>();
        _handler = new GetTimeSeriesStatisticsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidGranularity_ShouldReturnTimeSeriesStatistics()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 3);

        var timeSeriesData = new List<(DateTime Timestamp, int Total, int Successful, int Failed, double AvgResponseTimeMs, long TotalTokens, double AvgTokens)>
        {
            (new DateTime(2025, 1, 1), 100, 90, 10, 1500.5, 50000, 500.0),
            (new DateTime(2025, 1, 2), 120, 100, 20, 1600.5, 60000, 500.0),
            (new DateTime(2025, 1, 3), 80, 75, 5, 1400.5, 40000, 500.0)
        };

        _repositoryMock
            .Setup(x => x.GetTimeSeriesStatisticsAsync(
                agentId,
                startDate,
                endDate,
                "day",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeSeriesData);

        var query = new GetTimeSeriesStatistics
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate,
            Granularity = "day"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AgentId.Should().Be(agentId);
        result.Granularity.Should().Be("day");
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(endDate);
        result.DataPoints.Should().HaveCount(3);
        result.TotalExecutions.Should().Be(300);
        result.OverallSuccessRate.Should().BeApproximately(88.33, 0.01);
        result.OverallAverageResponseTimeMs.Should().BeApproximately(1500.5, 0.01);
        result.OverallTotalTokensUsed.Should().Be(150000);

        // Verify first data point
        var firstPoint = result.DataPoints[0];
        firstPoint.Timestamp.Should().Be(new DateTime(2025, 1, 1));
        firstPoint.TotalExecutions.Should().Be(100);
        firstPoint.SuccessfulExecutions.Should().Be(90);
        firstPoint.FailedExecutions.Should().Be(10);
        firstPoint.SuccessRate.Should().Be(90.0);
        firstPoint.AverageResponseTimeMs.Should().Be(1500.5);
        firstPoint.TotalTokensUsed.Should().Be(50000);
        firstPoint.AverageTokensPerExecution.Should().Be(500.0);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("unknown")]
    public async Task Handle_WithInvalidGranularity_ShouldThrowArgumentException(string granularity)
    {
        // Arrange
        var query = new GetTimeSeriesStatistics
        {
            AgentId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Granularity = granularity
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _handler.Handle(query, CancellationToken.None));
    }

    [Theory]
    [InlineData("hour")]
    [InlineData("day")]
    [InlineData("week")]
    [InlineData("month")]
    public async Task Handle_WithAllValidGranularities_ShouldSucceed(string granularity)
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var timeSeriesData = new List<(DateTime, int, int, int, double, long, double)>
        {
            (DateTime.UtcNow, 100, 90, 10, 1500.5, 50000, 500.0)
        };

        _repositoryMock
            .Setup(x => x.GetTimeSeriesStatisticsAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeSeriesData);

        var query = new GetTimeSeriesStatistics
        {
            AgentId = agentId,
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Granularity = granularity
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Granularity.Should().Be(granularity.ToLowerInvariant());
    }

    [Fact]
    public async Task Handle_WithNoData_ShouldReturnEmptyStatistics()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var timeSeriesData = new List<(DateTime, int, int, int, double, long, double)>();

        _repositoryMock
            .Setup(x => x.GetTimeSeriesStatisticsAsync(
                agentId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeSeriesData);

        var query = new GetTimeSeriesStatistics
        {
            AgentId = agentId,
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Granularity = "day"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DataPoints.Should().BeEmpty();
        result.TotalExecutions.Should().Be(0);
        result.OverallSuccessRate.Should().Be(0);
        result.OverallAverageResponseTimeMs.Should().Be(0);
        result.OverallTotalTokensUsed.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithCaseInsensitiveGranularity_ShouldNormalizeToLowerCase()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var timeSeriesData = new List<(DateTime, int, int, int, double, long, double)>
        {
            (DateTime.UtcNow, 100, 90, 10, 1500.5, 50000, 500.0)
        };

        _repositoryMock
            .Setup(x => x.GetTimeSeriesStatisticsAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeSeriesData);

        var query = new GetTimeSeriesStatistics
        {
            AgentId = agentId,
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Granularity = "DAY"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Granularity.Should().Be("day");
    }
}
