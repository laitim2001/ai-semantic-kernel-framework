using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents a plugin that can be added to agents
/// </summary>
public sealed class Plugin : BaseEntity
{
    private readonly List<AgentPlugin> _agentPlugins = new();

    /// <summary>
    /// Plugin name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Plugin type
    /// </summary>
    public PluginType Type { get; private set; }

    /// <summary>
    /// Plugin version
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Default configuration (JSON)
    /// </summary>
    public string? Configuration { get; private set; }

    /// <summary>
    /// Whether the plugin is enabled globally
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Plugin author/creator
    /// </summary>
    public string? Author { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<AgentPlugin> AgentPlugins => _agentPlugins.AsReadOnly();

    // Private constructor for EF Core
    private Plugin()
    {
        Name = string.Empty;
        Version = string.Empty;
        IsEnabled = true;
    }

    /// <summary>
    /// Creates a new plugin
    /// </summary>
    public static Plugin Create(
        string name,
        PluginType type,
        string version,
        string? description = null,
        string? configuration = null,
        string? author = null)
    {
        ValidateName(name);
        ValidateVersion(version);

        return new Plugin
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Type = type,
            Version = version.Trim(),
            Configuration = configuration,
            Author = author?.Trim(),
            IsEnabled = true
        };
    }

    /// <summary>
    /// Updates plugin information
    /// </summary>
    public void Update(
        string name,
        string? description,
        string? configuration)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description?.Trim();
        Configuration = configuration;
        MarkAsModified();
    }

    /// <summary>
    /// Updates plugin version
    /// </summary>
    public void UpdateVersion(string version)
    {
        ValidateVersion(version);
        Version = version.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Enables the plugin
    /// </summary>
    public void Enable()
    {
        if (IsEnabled)
            return;

        IsEnabled = true;
        MarkAsModified();
    }

    /// <summary>
    /// Disables the plugin
    /// </summary>
    public void Disable()
    {
        if (!IsEnabled)
            return;

        IsEnabled = false;
        MarkAsModified();
    }

    // Validation methods
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plugin name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Plugin name cannot exceed 100 characters", nameof(name));
    }

    private static void ValidateVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Plugin version cannot be empty", nameof(version));

        if (version.Length > 20)
            throw new ArgumentException("Plugin version cannot exceed 20 characters", nameof(version));
    }
}
