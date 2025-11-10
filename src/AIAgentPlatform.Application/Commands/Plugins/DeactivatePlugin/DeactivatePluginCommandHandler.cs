using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.DeactivatePlugin;

/// <summary>
/// Handler for DeactivatePluginCommand
/// </summary>
public sealed class DeactivatePluginCommandHandler : IRequestHandler<DeactivatePluginCommand, Unit>
{
    private readonly IPluginRepository _pluginRepository;

    public DeactivatePluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public async Task<Unit> Handle(DeactivatePluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetByIdAsync(request.Id, cancellationToken);

        if (plugin == null)
        {
            throw new InvalidOperationException($"Plugin with ID '{request.Id}' not found.");
        }

        plugin.Deactivate();

        await _pluginRepository.UpdateAsync(plugin, cancellationToken);

        return Unit.Value;
    }
}
