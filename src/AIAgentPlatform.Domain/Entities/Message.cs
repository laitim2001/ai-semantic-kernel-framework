using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// 訊息 Domain Entity
/// 代表對話中的單一訊息 (用戶或 AI 助理的回應)
/// </summary>
public class Message : BaseEntity
{
    // 必要屬性
    public Guid ConversationId { get; private set; }
    public MessageRole Role { get; private set; } = MessageRole.User;
    public string Content { get; private set; } = string.Empty;

    // 導航屬性
    public Conversation? Conversation { get; private set; }

    // 統計資訊
    public int TokenCount { get; private set; }

    // Private constructor for EF Core
    private Message() { }

    // Factory method for creating new Message
    public static Message Create(
        Guid conversationId,
        MessageRole role,
        string content)
    {
        if (conversationId == Guid.Empty)
        {
            throw new ArgumentException("Conversation ID 不能為空", nameof(conversationId));
        }

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role), "訊息角色不能為空");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("訊息內容不能為空", nameof(content));
        }

        if (content.Length > 32000)
        {
            throw new ArgumentException("訊息內容不能超過 32000 個字元", nameof(content));
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Role = role,
            Content = content.Trim(),
            TokenCount = EstimateTokenCount(content),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return message;
    }

    // 業務方法
    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
        {
            throw new ArgumentException("訊息內容不能為空", nameof(newContent));
        }

        if (newContent.Length > 32000)
        {
            throw new ArgumentException("訊息內容不能超過 32000 個字元", nameof(newContent));
        }

        Content = newContent.Trim();
        TokenCount = EstimateTokenCount(newContent);
        UpdatedAt = DateTime.UtcNow;
    }

    // Token 估算 (簡化版本，實際可能需要更精確的計算)
    private static int EstimateTokenCount(string content)
    {
        // 粗略估算: 中文約1.5字元/token，英文約4字元/token
        // 這裡使用簡化計算: 平均 2 字元 = 1 token
        return (content.Length + 1) / 2;
    }

    // 驗證訊息是否可以執行操作
    public bool CanUpdate() => true; // 訊息可以更新內容
    public bool IsUserMessage() => Role.IsUser;
    public bool IsAssistantMessage() => Role.IsAssistant;
    public bool IsSystemMessage() => Role.IsSystem;
}
