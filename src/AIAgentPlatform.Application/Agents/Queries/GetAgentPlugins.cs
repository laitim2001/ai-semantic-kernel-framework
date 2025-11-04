using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// 查詢 Agent 的所有 Plugins
/// </summary>
public record GetAgentPlugins(
    Guid AgentId,
    bool? EnabledOnly = null
) : IRequest<List<AgentPluginDto>>;
