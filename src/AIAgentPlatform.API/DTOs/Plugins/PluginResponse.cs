namespace AIAgentPlatform.API.DTOs.Plugins;

/// <summary>
/// Response model for plugin information
/// </summary>
public sealed record PluginResponse
{
    /// <summary>
    /// Plugin unique identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User ID who owns the plugin
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Plugin identifier
    /// </summary>
    public string PluginId { get; init; } = string.Empty;

    /// <summary>
    /// Plugin display name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Plugin version
    /// </summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Plugin category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Plugin status (Active, Inactive, Deprecated)
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Available functions in this plugin
    /// </summary>
    public List<PluginFunctionResponse> Functions { get; init; } = new();

    /// <summary>
    /// Plugin registration date
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Response model for plugin function information
/// </summary>
public sealed record PluginFunctionResponse
{
    /// <summary>
    /// Function name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Function description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Return type
    /// </summary>
    public string ReturnType { get; init; } = string.Empty;

    /// <summary>
    /// Function parameters
    /// </summary>
    public List<PluginParameterResponse> Parameters { get; init; } = new();
}

/// <summary>
/// Response model for plugin function parameter information
/// </summary>
public sealed record PluginParameterResponse
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Parameter type
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Parameter description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether this parameter is required
    /// </summary>
    public bool IsRequired { get; init; }
}
