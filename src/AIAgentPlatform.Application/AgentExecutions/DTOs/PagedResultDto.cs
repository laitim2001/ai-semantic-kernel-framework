namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// Paged result wrapper for paginated queries
/// </summary>
public sealed class PagedResultDto<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageSize { get; init; }
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
}
