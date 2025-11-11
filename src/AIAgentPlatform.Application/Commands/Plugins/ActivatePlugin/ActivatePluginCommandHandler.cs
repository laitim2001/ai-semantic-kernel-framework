using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.ActivatePlugin;

/// <summary>
/// Handler for ActivatePluginCommand
/// </summary>
public sealed class ActivatePluginCommandHandler : IRequestHandler<ActivatePluginCommand, Unit>
{
    private readonly IPluginRepository _pluginRepository;

    public ActivatePluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public async Task<Unit> Handle(ActivatePluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetByIdAsync(request.Id, cancellationToken);

        if (plugin == null)
        {
            throw new InvalidOperationException($"Plugin with ID '{request.Id}' not found.");
        }

        plugin.Activate();

        await _pluginRepository.UpdateAsync(plugin, cancellationToken);

        return Unit.Value;
    }
}
