namespace AIAgentPlatform.API.DTOs.Plugins;

/// <summary>
/// Request model for plugin update
/// </summary>
public sealed record UpdatePluginRequest
{
    /// <summary>
    /// Updated plugin name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Updated plugin description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated plugin category
    /// </summary>
    public string? Category { get; init; }
}
