using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Application.Common.DTOs;

/// <summary>
/// 訊息 DTO
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static MessageDto FromEntity(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Role = message.Role.Value,
            Content = message.Content,
            TokenCount = message.TokenCount,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }
}
