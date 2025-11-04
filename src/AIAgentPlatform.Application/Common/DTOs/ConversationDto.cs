using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Application.Common.DTOs;

/// <summary>
/// 對話 DTO
/// </summary>
public class ConversationDto
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static ConversationDto FromEntity(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            AgentId = conversation.AgentId,
            UserId = conversation.UserId,
            Title = conversation.Title,
            Status = conversation.Status.Value,
            MessageCount = conversation.MessageCount,
            LastMessageAt = conversation.LastMessageAt,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt
        };
    }
}
