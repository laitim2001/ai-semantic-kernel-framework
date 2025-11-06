using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get execution history for an agent with advanced filtering
/// </summary>
public sealed record GetExecutionHistory : IRequest<PagedResultDto<AgentExecutionDto>>
{
    public Guid AgentId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Status { get; init; }
    public Guid? ConversationId { get; init; }
    public int? MinTokens { get; init; }
    public int? MaxTokens { get; init; }
    public double? MinResponseTimeMs { get; init; }
    public double? MaxResponseTimeMs { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
}
