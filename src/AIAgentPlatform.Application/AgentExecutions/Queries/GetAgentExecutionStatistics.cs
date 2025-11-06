using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get execution statistics for an agent
/// </summary>
public sealed record GetAgentExecutionStatistics : IRequest<AgentExecutionStatisticsDto>
{
    public Guid AgentId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
