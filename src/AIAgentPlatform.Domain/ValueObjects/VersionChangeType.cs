namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents the type of version change
/// </summary>
public sealed class VersionChangeType
{
    public string Value { get; }

    // Predefined change types
    public static readonly VersionChangeType Major = new("major");
    public static readonly VersionChangeType Minor = new("minor");
    public static readonly VersionChangeType Patch = new("patch");
    public static readonly VersionChangeType Rollback = new("rollback");
    public static readonly VersionChangeType Hotfix = new("hotfix");

    private static readonly HashSet<string> ValidTypes = new()
    {
        "major",
        "minor",
        "patch",
        "rollback",
        "hotfix"
    };

    private VersionChangeType(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new VersionChangeType from string value
    /// </summary>
    public static VersionChangeType From(string value)
    {
        var normalizedValue = string.IsNullOrWhiteSpace(value) ? value : value.ToLowerInvariant();

        if (!ValidTypes.Contains(normalizedValue))
            throw new ArgumentException($"Invalid version change type: {value ?? "(null)"}. Valid types: {string.Join(", ", ValidTypes)}", nameof(value));

        return new VersionChangeType(normalizedValue);
    }

    /// <summary>
    /// Checks if a change type string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidTypes.Contains(value.ToLowerInvariant());
    }

    public bool IsMajor => Value == "major";
    public bool IsMinor => Value == "minor";
    public bool IsPatch => Value == "patch";
    public bool IsRollback => Value == "rollback";
    public bool IsHotfix => Value == "hotfix";

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is VersionChangeType other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(VersionChangeType type) => type.Value;
}
