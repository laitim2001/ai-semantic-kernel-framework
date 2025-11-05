namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents the type of plugin
/// </summary>
public sealed class PluginType
{
    public string Value { get; }

    // Predefined plugin types
    public static readonly PluginType Tool = new("tool");
    public static readonly PluginType Function = new("function");
    public static readonly PluginType Skill = new("skill");
    public static readonly PluginType Connector = new("connector");
    public static readonly PluginType Memory = new("memory");
    public static readonly PluginType Custom = new("custom");

    private static readonly HashSet<string> ValidTypes = new()
    {
        "tool",
        "function",
        "skill",
        "connector",
        "memory",
        "custom"
    };

    private PluginType(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new PluginType from string value
    /// </summary>
    public static PluginType From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plugin type cannot be empty", nameof(value));

        var normalizedValue = value.ToLowerInvariant();

        if (!ValidTypes.Contains(normalizedValue))
            throw new ArgumentException($"Invalid plugin type: {value}. Valid types: {string.Join(", ", ValidTypes)}", nameof(value));

        return new PluginType(normalizedValue);
    }

    /// <summary>
    /// Checks if a type string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidTypes.Contains(value.ToLowerInvariant());
    }

    public bool IsTool => Value == "tool";
    public bool IsFunction => Value == "function";
    public bool IsSkill => Value == "skill";
    public bool IsConnector => Value == "connector";
    public bool IsMemory => Value == "memory";
    public bool IsCustom => Value == "custom";

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is PluginType other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(PluginType type) => type.Value;
}
