using AIAgentPlatform.Application.Common.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Commands.AddMessage;

/// <summary>
/// 新增訊息 Command
/// </summary>
public record AddMessageCommand : IRequest<MessageDto>
{
    public Guid ConversationId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
