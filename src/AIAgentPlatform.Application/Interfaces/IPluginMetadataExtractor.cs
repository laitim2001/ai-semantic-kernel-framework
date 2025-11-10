using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Application.Interfaces;

/// <summary>
/// Service for extracting plugin metadata from assemblies using reflection
/// </summary>
public interface IPluginMetadataExtractor
{
    /// <summary>
    /// Extracts plugin metadata from an assembly file
    /// </summary>
    /// <param name="assemblyPath">Path to the plugin assembly</param>
    /// <returns>Extracted plugin metadata including PluginId, Name, Version, Functions</returns>
    Task<PluginMetadataResult> ExtractFromAssemblyAsync(string assemblyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts plugin metadata from a loaded type
    /// </summary>
    /// <param name="pluginType">Plugin type to extract metadata from</param>
    /// <returns>Extracted plugin metadata</returns>
    PluginMetadataResult ExtractFromType(Type pluginType);

    /// <summary>
    /// Validates that a type is a valid plugin
    /// </summary>
    /// <param name="pluginType">Type to validate</param>
    /// <returns>True if valid plugin, false otherwise</returns>
    bool IsValidPlugin(Type pluginType);
}

/// <summary>
/// Result of plugin metadata extraction
/// </summary>
public sealed class PluginMetadataResult
{
    /// <summary>
    /// Plugin unique identifier
    /// </summary>
    public string PluginId { get; set; } = string.Empty;

    /// <summary>
    /// Plugin display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Plugin version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Plugin category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Plugin metadata (functions and parameters)
    /// </summary>
    public PluginMetadata Metadata { get; set; } = PluginMetadata.Empty;

    /// <summary>
    /// Assembly full name
    /// </summary>
    public string? AssemblyFullName { get; set; }

    /// <summary>
    /// Plugin type full name
    /// </summary>
    public string? TypeFullName { get; set; }

    /// <summary>
    /// Indicates if extraction was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if extraction failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static PluginMetadataResult Success(
        string pluginId,
        string name,
        string version,
        PluginMetadata metadata,
        string? description = null,
        string? category = null,
        string? assemblyFullName = null,
        string? typeFullName = null)
    {
        return new PluginMetadataResult
        {
            PluginId = pluginId,
            Name = name,
            Version = version,
            Description = description,
            Category = category,
            Metadata = metadata,
            AssemblyFullName = assemblyFullName,
            TypeFullName = typeFullName,
            IsSuccess = true
        };
    }

    /// <summary>
    /// Creates a failure result
    /// </summary>
    public static PluginMetadataResult Failure(string errorMessage)
    {
        return new PluginMetadataResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
