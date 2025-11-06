namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// DTO for agent execution statistics
/// </summary>
public sealed class AgentExecutionStatisticsDto
{
    public Guid AgentId { get; init; }
    public int TotalExecutions { get; init; }
    public int SuccessfulExecutions { get; init; }
    public int FailedExecutions { get; init; }
    public double SuccessRate { get; init; }
    public double AverageResponseTimeMs { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
