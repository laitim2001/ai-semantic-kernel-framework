using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to add a plugin to an agent
/// </summary>
public sealed record AddPluginToAgentCommand : IRequest<bool>
{
    public Guid AgentId { get; init; }
    public Guid PluginId { get; init; }
    public Guid UserId { get; init; }
    public int ExecutionOrder { get; init; } = 0;
    public string? CustomConfiguration { get; init; }
}
