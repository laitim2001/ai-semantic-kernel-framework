namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents the status of a Plugin
/// </summary>
public sealed class PluginStatus
{
    public string Value { get; }

    // Predefined statuses
    public static readonly PluginStatus Active = new("active");
    public static readonly PluginStatus Inactive = new("inactive");
    public static readonly PluginStatus Failed = new("failed");

    private static readonly HashSet<string> ValidStatuses = new()
    {
        "active",
        "inactive",
        "failed"
    };

    private PluginStatus(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new PluginStatus from string value
    /// </summary>
    public static PluginStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plugin status cannot be empty", nameof(value));

        var normalizedValue = value.ToLowerInvariant();

        if (!ValidStatuses.Contains(normalizedValue))
            throw new ArgumentException($"Invalid plugin status: {value}. Valid statuses: {string.Join(", ", ValidStatuses)}", nameof(value));

        return new PluginStatus(normalizedValue);
    }

    /// <summary>
    /// Checks if a status string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidStatuses.Contains(value.ToLowerInvariant());
    }

    public bool IsActive => Value == "active";
    public bool IsInactive => Value == "inactive";
    public bool IsFailed => Value == "failed";

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is PluginStatus other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(PluginStatus status) => status.Value;
}
