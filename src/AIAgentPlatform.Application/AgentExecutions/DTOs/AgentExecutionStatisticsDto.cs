namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// DTO for agent execution statistics with enhanced performance metrics
/// </summary>
public sealed class AgentExecutionStatisticsDto
{
    // Basic Statistics
    public Guid AgentId { get; init; }
    public int TotalExecutions { get; init; }
    public int SuccessfulExecutions { get; init; }
    public int FailedExecutions { get; init; }
    public int CancelledExecutions { get; init; }
    public double SuccessRate { get; init; }

    // Response Time Metrics
    public double AverageResponseTimeMs { get; init; }
    public double? MinResponseTimeMs { get; init; }
    public double? MaxResponseTimeMs { get; init; }
    public double? MedianResponseTimeMs { get; init; }
    public double? P95ResponseTimeMs { get; init; }
    public double? P99ResponseTimeMs { get; init; }

    // Token Usage Metrics
    public long TotalTokensUsed { get; init; }
    public double AverageTokensPerExecution { get; init; }
    public int? MinTokensUsed { get; init; }
    public int? MaxTokensUsed { get; init; }

    // Date Range
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
