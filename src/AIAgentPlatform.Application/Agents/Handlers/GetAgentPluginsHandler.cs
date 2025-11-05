using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent Plugins 查詢
/// </summary>
public class GetAgentPluginsHandler : IRequestHandler<GetAgentPlugins, List<AgentPluginDto>>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentPluginRepository _agentPluginRepository;

    public GetAgentPluginsHandler(
        IAgentRepository agentRepository,
        IAgentPluginRepository agentPluginRepository)
    {
        _agentRepository = agentRepository;
        _agentPluginRepository = agentPluginRepository;
    }

    public async Task<List<AgentPluginDto>> Handle(GetAgentPlugins request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // 查詢 Agent 的所有 Plugins
        var agentPlugins = await _agentPluginRepository.GetByAgentIdAsync(
            request.AgentId,
            request.EnabledOnly,
            cancellationToken);

        // 轉換為 DTO
        return agentPlugins.Select(ap => new AgentPluginDto
        {
            AgentId = ap.AgentId,
            PluginId = ap.PluginId,
            PluginName = ap.Plugin?.Name ?? string.Empty,
            PluginDescription = ap.Plugin?.Description,
            PluginType = ap.Plugin?.Type.Value ?? string.Empty,
            PluginVersion = ap.Plugin?.Version ?? string.Empty,
            IsEnabled = ap.IsEnabled,
            ExecutionOrder = ap.ExecutionOrder,
            CustomConfiguration = ap.CustomConfiguration,
            AddedAt = ap.AddedAt
        }).ToList();
    }
}
