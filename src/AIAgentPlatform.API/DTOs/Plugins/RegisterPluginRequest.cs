namespace AIAgentPlatform.API.DTOs.Plugins;

/// <summary>
/// Request model for plugin registration
/// </summary>
public sealed record RegisterPluginRequest
{
    /// <summary>
    /// User ID who owns the plugin
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Path to plugin assembly file (.dll or .exe) for automatic metadata extraction
    /// </summary>
    public string? AssemblyPath { get; init; }

    /// <summary>
    /// Plugin identifier (required if AssemblyPath is not provided)
    /// </summary>
    public string? PluginId { get; init; }

    /// <summary>
    /// Plugin display name (required if AssemblyPath is not provided)
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Plugin version in semantic versioning format (required if AssemblyPath is not provided)
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Plugin category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Automatically activate plugin after registration
    /// </summary>
    public bool AutoActivate { get; init; } = false;
}
