using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// 從 Agent 移除 Plugin
/// </summary>
public sealed class RemovePluginFromAgentCommand : IRequest<bool>
{
    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Plugin ID
    /// </summary>
    public Guid PluginId { get; set; }
}
