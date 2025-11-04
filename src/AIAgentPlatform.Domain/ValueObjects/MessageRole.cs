namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// 訊息角色 Value Object
/// 代表訊息的發送者角色 (User 或 Assistant)
/// </summary>
public sealed class MessageRole : IEquatable<MessageRole>
{
    public string Value { get; }

    // 預定義角色
    public static readonly MessageRole User = new("user");
    public static readonly MessageRole Assistant = new("assistant");
    public static readonly MessageRole System = new("system");

    private static readonly HashSet<string> ValidRoles = new()
    {
        "user",
        "assistant",
        "system"
    };

    private MessageRole(string value)
    {
        Value = value;
    }

    public static MessageRole Create(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("訊息角色不能為空", nameof(role));
        }

        var normalizedRole = role.ToLowerInvariant().Trim();

        if (!ValidRoles.Contains(normalizedRole))
        {
            throw new ArgumentException(
                $"無效的訊息角色: {role}. 有效角色: {string.Join(", ", ValidRoles)}",
                nameof(role));
        }

        return normalizedRole switch
        {
            "user" => User,
            "assistant" => Assistant,
            "system" => System,
            _ => throw new ArgumentException($"未知的訊息角色: {role}", nameof(role))
        };
    }

    public bool IsUser => Value == "user";
    public bool IsAssistant => Value == "assistant";
    public bool IsSystem => Value == "system";

    // IEquatable implementation
    public bool Equals(MessageRole? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as MessageRole);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(MessageRole? left, MessageRole? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(MessageRole? left, MessageRole? right)
        => !(left == right);
}
