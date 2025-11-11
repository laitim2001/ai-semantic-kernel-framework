using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.DeactivatePlugin;

/// <summary>
/// Command to deactivate a plugin
/// </summary>
public sealed record DeactivatePluginCommand : IRequest<Unit>
{
    /// <summary>
    /// Plugin ID to deactivate
    /// </summary>
    public Guid Id { get; init; }

    public DeactivatePluginCommand(Guid id)
    {
        Id = id;
    }
}
