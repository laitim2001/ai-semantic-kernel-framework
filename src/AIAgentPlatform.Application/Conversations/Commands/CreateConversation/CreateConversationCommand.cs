using AIAgentPlatform.Application.Common.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Commands.CreateConversation;

/// <summary>
/// 創建對話 Command
/// </summary>
public record CreateConversationCommand : IRequest<ConversationDto>
{
    public Guid AgentId { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
}
