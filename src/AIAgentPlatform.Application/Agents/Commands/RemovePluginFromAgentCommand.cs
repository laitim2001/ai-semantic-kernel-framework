using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to remove a plugin from an agent
/// </summary>
public sealed record RemovePluginFromAgentCommand : IRequest<bool>
{
    public Guid AgentId { get; init; }
    public Guid PluginId { get; init; }
}
