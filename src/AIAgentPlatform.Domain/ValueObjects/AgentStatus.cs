namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents the status of an Agent
/// </summary>
public sealed class AgentStatus
{
    public string Value { get; }

    // Predefined statuses
    public static readonly AgentStatus Active = new("active");
    public static readonly AgentStatus Paused = new("paused");
    public static readonly AgentStatus Archived = new("archived");

    private static readonly HashSet<string> ValidStatuses = new()
    {
        "active",
        "paused",
        "archived"
    };

    private AgentStatus(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new AgentStatus from string value
    /// </summary>
    public static AgentStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Agent status cannot be empty", nameof(value));

        var normalizedValue = value.ToLowerInvariant();

        if (!ValidStatuses.Contains(normalizedValue))
            throw new ArgumentException($"Invalid agent status: {value}. Valid statuses: {string.Join(", ", ValidStatuses)}", nameof(value));

        return new AgentStatus(normalizedValue);
    }

    /// <summary>
    /// Checks if a status string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidStatuses.Contains(value.ToLowerInvariant());
    }

    public bool IsActive => Value == "active";
    public bool IsPaused => Value == "paused";
    public bool IsArchived => Value == "archived";

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is AgentStatus other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(AgentStatus status) => status.Value;
}
