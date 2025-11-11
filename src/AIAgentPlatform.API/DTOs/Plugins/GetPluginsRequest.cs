using Microsoft.AspNetCore.Mvc;

namespace AIAgentPlatform.API.DTOs.Plugins;

/// <summary>
/// Request model for querying plugins
/// </summary>
public sealed record GetPluginsRequest
{
    /// <summary>
    /// Filter by user ID
    /// </summary>
    [FromQuery(Name = "userId")]
    public Guid? UserId { get; init; }

    /// <summary>
    /// Filter by status (Active, Inactive, Deprecated)
    /// </summary>
    [FromQuery(Name = "status")]
    public string? Status { get; init; }

    /// <summary>
    /// Filter by category
    /// </summary>
    [FromQuery(Name = "category")]
    public string? Category { get; init; }

    /// <summary>
    /// Search term for name, description, or plugin ID
    /// </summary>
    [FromQuery(Name = "search")]
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Sort field (Name, CreatedAt, UpdatedAt)
    /// </summary>
    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    [FromQuery(Name = "sortDir")]
    public string? SortDirection { get; init; }

    /// <summary>
    /// Number of records to skip
    /// </summary>
    [FromQuery(Name = "skip")]
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Number of records to take (max 100)
    /// </summary>
    [FromQuery(Name = "take")]
    public int Take { get; init; } = 50;
}
