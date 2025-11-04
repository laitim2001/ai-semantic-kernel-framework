using AIAgentPlatform.Application.Common.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Queries.GetConversationById;

/// <summary>
/// 取得對話詳情 Query
/// </summary>
public record GetConversationByIdQuery : IRequest<ConversationDto?>
{
    public Guid Id { get; init; }
}
