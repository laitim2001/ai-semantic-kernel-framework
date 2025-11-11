using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.UpdatePlugin;

/// <summary>
/// Handler for UpdatePluginCommand
/// </summary>
public sealed class UpdatePluginCommandHandler : IRequestHandler<UpdatePluginCommand, Unit>
{
    private readonly IPluginRepository _pluginRepository;

    public UpdatePluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public async Task<Unit> Handle(UpdatePluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetByIdAsync(request.Id, cancellationToken);

        if (plugin == null)
        {
            throw new InvalidOperationException($"Plugin with ID '{request.Id}' not found.");
        }

        // Update plugin information
        // If name is not provided, use existing name
        plugin.Update(
            name: request.Name ?? plugin.Name,
            description: request.Description,
            category: request.Category);

        await _pluginRepository.UpdateAsync(plugin, cancellationToken);

        return Unit.Value;
    }
}
