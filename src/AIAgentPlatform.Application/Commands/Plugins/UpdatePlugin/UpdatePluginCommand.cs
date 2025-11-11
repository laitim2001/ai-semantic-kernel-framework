using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.UpdatePlugin;

/// <summary>
/// Command to update plugin information
/// </summary>
public sealed record UpdatePluginCommand : IRequest<Unit>
{
    /// <summary>
    /// Plugin ID to update
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Updated plugin name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Updated description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated category
    /// </summary>
    public string? Category { get; init; }
}
