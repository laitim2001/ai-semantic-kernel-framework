using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get time-series execution statistics for an agent
/// </summary>
public sealed record GetTimeSeriesStatistics : IRequest<TimeSeriesStatisticsDto>
{
    public Guid AgentId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Granularity { get; init; } = "day"; // "hour", "day", "week", "month"
}
