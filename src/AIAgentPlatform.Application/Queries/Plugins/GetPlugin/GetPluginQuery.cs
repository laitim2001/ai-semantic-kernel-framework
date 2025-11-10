using AIAgentPlatform.Domain.Entities;
using MediatR;

namespace AIAgentPlatform.Application.Queries.Plugins.GetPlugin;

/// <summary>
/// Query to get a single plugin by ID
/// </summary>
public sealed record GetPluginQuery : IRequest<Plugin?>
{
    /// <summary>
    /// Plugin ID (GUID)
    /// </summary>
    public Guid Id { get; init; }

    public GetPluginQuery(Guid id)
    {
        Id = id;
    }
}
