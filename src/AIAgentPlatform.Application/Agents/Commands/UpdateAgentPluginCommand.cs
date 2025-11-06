using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to update agent plugin configuration
/// </summary>
public sealed record UpdateAgentPluginCommand : IRequest<bool>
{
    public Guid AgentId { get; init; }
    public Guid PluginId { get; init; }
    public bool IsEnabled { get; init; }
    public int ExecutionOrder { get; init; }
    public string? CustomConfiguration { get; init; }
}
