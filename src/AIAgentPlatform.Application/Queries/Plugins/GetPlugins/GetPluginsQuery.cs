using AIAgentPlatform.Domain.Entities;
using MediatR;

namespace AIAgentPlatform.Application.Queries.Plugins.GetPlugins;

/// <summary>
/// Query to get plugins with filtering and pagination
/// </summary>
public sealed record GetPluginsQuery : IRequest<List<Plugin>>
{
    /// <summary>
    /// Filter by user ID
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Search term for plugin name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by plugin status (active, inactive, failed)
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Number of records to skip
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Number of records to take (max 100)
    /// </summary>
    public int Take { get; init; } = 50;

    /// <summary>
    /// Sort field (name, created, updated)
    /// </summary>
    public string SortBy { get; init; } = "created";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; init; } = "desc";
}
