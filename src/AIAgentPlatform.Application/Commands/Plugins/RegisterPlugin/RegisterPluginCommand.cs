using AIAgentPlatform.Domain.Entities;
using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.RegisterPlugin;

/// <summary>
/// Command to register a new plugin from an assembly file or direct metadata
/// </summary>
public sealed record RegisterPluginCommand : IRequest<Plugin>
{
    /// <summary>
    /// User ID that owns this plugin
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Path to the plugin assembly file (if loading from file)
    /// </summary>
    public string? AssemblyPath { get; init; }

    /// <summary>
    /// Plugin ID (required if not loading from assembly)
    /// </summary>
    public string? PluginId { get; init; }

    /// <summary>
    /// Plugin name (required if not loading from assembly)
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Plugin version (required if not loading from assembly)
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
    /// Whether to auto-activate the plugin after registration
    /// </summary>
    public bool AutoActivate { get; init; } = false;
}
