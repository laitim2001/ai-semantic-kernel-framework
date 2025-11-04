using AIAgentPlatform.Application.Common.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Queries.GetConversations;

/// <summary>
/// 取得對話列表 Query
/// </summary>
public record GetConversationsQuery : IRequest<List<ConversationDto>>
{
    public Guid? UserId { get; init; }
    public Guid? AgentId { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
