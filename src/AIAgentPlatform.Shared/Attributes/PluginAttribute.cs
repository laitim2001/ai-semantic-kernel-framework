namespace AIAgentPlatform.Shared.Attributes;

/// <summary>
/// Marks a class as a plugin that can be registered with the platform
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PluginAttribute : Attribute
{
    /// <summary>
    /// Unique identifier for the plugin
    /// </summary>
    public string PluginId { get; }

    /// <summary>
    /// Plugin version following semantic versioning
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Plugin display name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Plugin category for organization
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Creates a new PluginAttribute
    /// </summary>
    /// <param name="pluginId">Unique plugin identifier</param>
    /// <param name="version">Plugin version (e.g., "1.0.0")</param>
    public PluginAttribute(string pluginId, string version)
    {
        if (string.IsNullOrWhiteSpace(pluginId))
            throw new ArgumentException("Plugin ID cannot be empty", nameof(pluginId));

        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Plugin version cannot be empty", nameof(version));

        PluginId = pluginId;
        Version = version;
    }
}
