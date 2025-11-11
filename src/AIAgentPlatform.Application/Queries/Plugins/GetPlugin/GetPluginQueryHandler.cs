using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Queries.Plugins.GetPlugin;

/// <summary>
/// Handler for GetPluginQuery
/// </summary>
public sealed class GetPluginQueryHandler : IRequestHandler<GetPluginQuery, Plugin?>
{
    private readonly IPluginRepository _pluginRepository;

    public GetPluginQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public async Task<Plugin?> Handle(GetPluginQuery request, CancellationToken cancellationToken)
    {
        return await _pluginRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
