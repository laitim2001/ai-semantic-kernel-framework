using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// 對話 Domain Entity
/// 代表用戶與 Agent 之間的一次對話會話
/// </summary>
public class Conversation : BaseEntity
{
    private readonly List<Message> _messages = new();

    // 必要屬性
    public Guid AgentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public ConversationStatus Status { get; private set; } = ConversationStatus.Active;

    // 導航屬性
    public Agent? Agent { get; private set; }
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    // 統計資訊
    public int MessageCount { get; private set; }
    public DateTime? LastMessageAt { get; private set; }

    // Private constructor for EF Core
    private Conversation() { }

    // Factory method for creating new Conversation
    public static Conversation Create(
        Guid agentId,
        Guid userId,
        string title)
    {
        if (agentId == Guid.Empty)
        {
            throw new ArgumentException("Agent ID 不能為空", nameof(agentId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID 不能為空", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("對話標題不能為空", nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException("對話標題不能超過 200 個字元", nameof(title));
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            UserId = userId,
            Title = title.Trim(),
            Status = ConversationStatus.Active,
            MessageCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return conversation;
    }

    // 業務方法
    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("對話標題不能為空", nameof(newTitle));
        }

        if (newTitle.Length > 200)
        {
            throw new ArgumentException("對話標題不能超過 200 個字元", nameof(newTitle));
        }

        Title = newTitle.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status == ConversationStatus.Deleted)
        {
            throw new InvalidOperationException("已刪除的對話無法封存");
        }

        Status = ConversationStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == ConversationStatus.Deleted)
        {
            throw new InvalidOperationException("已刪除的對話無法重新啟用");
        }

        Status = ConversationStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        Status = ConversationStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMessage(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (Status != ConversationStatus.Active)
        {
            throw new InvalidOperationException("只有活躍的對話可以新增訊息");
        }

        _messages.Add(message);
        MessageCount++;
        LastMessageAt = message.CreatedAt;
        UpdatedAt = DateTime.UtcNow;
    }

    // 驗證對話是否可以執行操作
    public bool CanAddMessage() => Status == ConversationStatus.Active;
    public bool CanUpdate() => Status != ConversationStatus.Deleted;
    public bool CanDelete() => Status != ConversationStatus.Deleted;
}
