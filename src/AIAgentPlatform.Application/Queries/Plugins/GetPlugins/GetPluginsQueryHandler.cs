using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Queries.Plugins.GetPlugins;

/// <summary>
/// Handler for GetPluginsQuery
/// </summary>
public sealed class GetPluginsQueryHandler : IRequestHandler<GetPluginsQuery, List<Plugin>>
{
    private readonly IPluginRepository _pluginRepository;

    public GetPluginsQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public async Task<List<Plugin>> Handle(GetPluginsQuery request, CancellationToken cancellationToken)
    {
        // Limit Take to maximum 100
        var take = Math.Min(request.Take, 100);

        return await _pluginRepository.GetAllAsync(
            userId: request.UserId,
            status: request.Status,
            category: request.Category,
            searchTerm: request.SearchTerm,
            sortBy: request.SortBy,
            sortOrder: request.SortDirection,
            skip: request.Skip,
            take: take,
            cancellationToken: cancellationToken);
    }
}
