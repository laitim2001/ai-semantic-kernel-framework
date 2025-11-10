using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.ActivatePlugin;

/// <summary>
/// Command to activate a plugin
/// </summary>
public sealed record ActivatePluginCommand : IRequest<Unit>
{
    /// <summary>
    /// Plugin ID to activate
    /// </summary>
    public Guid Id { get; init; }

    public ActivatePluginCommand(Guid id)
    {
        Id = id;
    }
}
