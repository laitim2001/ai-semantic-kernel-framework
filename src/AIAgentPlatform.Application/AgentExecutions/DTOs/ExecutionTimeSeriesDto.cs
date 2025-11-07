namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// DTO for time-series execution statistics
/// </summary>
public sealed class ExecutionTimeSeriesDto
{
    public DateTime Timestamp { get; init; }
    public int TotalExecutions { get; init; }
    public int SuccessfulExecutions { get; init; }
    public int FailedExecutions { get; init; }
    public double SuccessRate { get; init; }
    public double AverageResponseTimeMs { get; init; }
    public long TotalTokensUsed { get; init; }
    public double AverageTokensPerExecution { get; init; }
}

/// <summary>
/// DTO for aggregated time-series statistics response
/// </summary>
public sealed class TimeSeriesStatisticsDto
{
    public Guid AgentId { get; init; }
    public string Granularity { get; init; } = string.Empty; // "hour", "day", "week", "month"
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public List<ExecutionTimeSeriesDto> DataPoints { get; init; } = new();

    // Overall summary
    public int TotalExecutions { get; init; }
    public double OverallSuccessRate { get; init; }
    public double OverallAverageResponseTimeMs { get; init; }
    public long OverallTotalTokensUsed { get; init; }
}
