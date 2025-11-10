using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents a .NET Plugin with metadata and configuration
/// </summary>
public sealed class Plugin : BaseEntity
{
    /// <summary>
    /// Owner of the plugin
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Unique plugin identifier (e.g., "WeatherPlugin")
    /// </summary>
    public string PluginId { get; private set; }

    /// <summary>
    /// Plugin display name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Plugin category for organization
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// Plugin version (e.g., "1.0.0")
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Plugin metadata extracted from assembly
    /// </summary>
    public PluginMetadata Metadata { get; private set; }

    /// <summary>
    /// Current status of the plugin
    /// </summary>
    public PluginStatus Status { get; private set; }

    /// <summary>
    /// Assembly file path or location
    /// </summary>
    public string? AssemblyPath { get; private set; }

    /// <summary>
    /// Assembly full name for loading
    /// </summary>
    public string? AssemblyFullName { get; private set; }

    // Private constructor for EF Core
    private Plugin()
    {
        PluginId = string.Empty;
        Name = string.Empty;
        Version = "1.0.0";
        Metadata = PluginMetadata.Empty;
        Status = PluginStatus.Active;
    }

    /// <summary>
    /// Creates a new Plugin instance
    /// </summary>
    public static Plugin Create(
        Guid userId,
        string pluginId,
        string name,
        string version,
        PluginMetadata metadata,
        string? description = null,
        string? category = null,
        string? assemblyPath = null,
        string? assemblyFullName = null)
    {
        ValidatePluginId(pluginId);
        ValidateName(name);
        ValidateVersion(version);

        return new Plugin
        {
            UserId = userId,
            PluginId = pluginId.Trim(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Category = category?.Trim(),
            Version = version.Trim(),
            Metadata = metadata,
            Status = PluginStatus.Active,
            AssemblyPath = assemblyPath?.Trim(),
            AssemblyFullName = assemblyFullName?.Trim()
        };
    }

    /// <summary>
    /// Updates plugin basic information
    /// </summary>
    public void Update(string name, string? description, string? category)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description?.Trim();
        Category = category?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Updates plugin version and metadata
    /// </summary>
    public void UpdateVersionAndMetadata(string version, PluginMetadata metadata)
    {
        ValidateVersion(version);

        Version = version.Trim();
        Metadata = metadata;
        MarkAsModified();
    }

    /// <summary>
    /// Updates plugin assembly information
    /// </summary>
    public void UpdateAssemblyInfo(string? assemblyPath, string? assemblyFullName)
    {
        AssemblyPath = assemblyPath?.Trim();
        AssemblyFullName = assemblyFullName?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Activates the plugin
    /// </summary>
    public void Activate()
    {
        if (Status.IsActive)
            return;

        Status = PluginStatus.Active;
        MarkAsModified();
    }

    /// <summary>
    /// Deactivates the plugin
    /// </summary>
    public void Deactivate()
    {
        if (Status.IsInactive)
            return;

        Status = PluginStatus.Inactive;
        MarkAsModified();
    }

    /// <summary>
    /// Marks plugin as failed with error
    /// </summary>
    public void MarkAsFailed(string? errorMessage = null)
    {
        Status = PluginStatus.Failed;
        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Description = errorMessage;
        }
        MarkAsModified();
    }

    // Validation methods
    private static void ValidatePluginId(string pluginId)
    {
        if (string.IsNullOrWhiteSpace(pluginId))
            throw new ArgumentException("Plugin ID cannot be empty", nameof(pluginId));

        if (pluginId.Length > 100)
            throw new ArgumentException("Plugin ID cannot exceed 100 characters", nameof(pluginId));

        // Plugin ID should be alphanumeric with underscores/hyphens only
        if (!System.Text.RegularExpressions.Regex.IsMatch(pluginId, @"^[a-zA-Z0-9_-]+$"))
            throw new ArgumentException("Plugin ID can only contain letters, numbers, underscores, and hyphens", nameof(pluginId));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plugin name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Plugin name cannot exceed 200 characters", nameof(name));
    }

    private static void ValidateVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Plugin version cannot be empty", nameof(version));

        if (version.Length > 20)
            throw new ArgumentException("Plugin version cannot exceed 20 characters", nameof(version));

        // Version should follow semantic versioning pattern
        if (!System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?$"))
            throw new ArgumentException("Plugin version must follow semantic versioning (e.g., 1.0.0)", nameof(version));
    }
}
