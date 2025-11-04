namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// 對話狀態 Value Object
/// </summary>
public sealed class ConversationStatus : IEquatable<ConversationStatus>
{
    public string Value { get; }

    // 預定義狀態
    public static readonly ConversationStatus Active = new("active");
    public static readonly ConversationStatus Archived = new("archived");
    public static readonly ConversationStatus Deleted = new("deleted");

    private static readonly HashSet<string> ValidStatuses = new()
    {
        "active",
        "archived",
        "deleted"
    };

    private ConversationStatus(string value)
    {
        Value = value;
    }

    public static ConversationStatus Create(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("對話狀態不能為空", nameof(status));
        }

        var normalizedStatus = status.ToLowerInvariant().Trim();

        if (!ValidStatuses.Contains(normalizedStatus))
        {
            throw new ArgumentException(
                $"無效的對話狀態: {status}. 有效狀態: {string.Join(", ", ValidStatuses)}",
                nameof(status));
        }

        return normalizedStatus switch
        {
            "active" => Active,
            "archived" => Archived,
            "deleted" => Deleted,
            _ => throw new ArgumentException($"未知的對話狀態: {status}", nameof(status))
        };
    }

    public bool IsActive => Value == "active";
    public bool IsArchived => Value == "archived";
    public bool IsDeleted => Value == "deleted";

    // IEquatable implementation
    public bool Equals(ConversationStatus? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as ConversationStatus);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(ConversationStatus? left, ConversationStatus? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ConversationStatus? left, ConversationStatus? right)
        => !(left == right);
}
