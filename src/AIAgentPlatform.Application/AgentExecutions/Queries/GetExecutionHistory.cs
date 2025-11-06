using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get execution history for an agent
/// </summary>
public sealed record GetExecutionHistory : IRequest<List<AgentExecutionDto>>
{
    public Guid AgentId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Status { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
}
