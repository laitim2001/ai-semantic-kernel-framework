using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// Query to get a list of agents with pagination and filtering
/// </summary>
public sealed record GetAgentsQuery : IRequest<GetAgentsQueryResult>
{
    public Guid? UserId { get; init; }
    public string? Status { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
}

/// <summary>
/// Result for GetAgentsQuery with pagination metadata
/// </summary>
public sealed record GetAgentsQueryResult
{
    public List<AgentDto> Agents { get; init; } = new();
    public int TotalCount { get; init; }
    public int Skip { get; init; }
    public int Take { get; init; }
}
