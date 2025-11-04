using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// Query to get a list of agents with pagination, filtering, searching, and sorting
/// </summary>
public sealed record GetAgentsQuery : IRequest<GetAgentsQueryResult>
{
    /// <summary>
    /// Filter by user ID
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Filter by agent status (active, paused, archived)
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Search term for name, description, or ID
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by model name (e.g., gpt-4, gpt-4o, gpt-4o-mini)
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Sort field (name, createdAt, updatedAt)
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort order (asc, desc). Default: desc
    /// </summary>
    public string? SortOrder { get; init; }

    /// <summary>
    /// Number of records to skip (for pagination)
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Number of records to take (for pagination). Max: 100
    /// </summary>
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
