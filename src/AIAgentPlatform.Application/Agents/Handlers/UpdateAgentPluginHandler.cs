using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理更新 Agent Plugin 設定
/// </summary>
public class UpdateAgentPluginHandler : IRequestHandler<UpdateAgentPluginCommand, bool>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly IAgentPluginRepository _agentPluginRepository;

    public UpdateAgentPluginHandler(
        IAgentRepository agentRepository,
        IPluginRepository pluginRepository,
        IAgentPluginRepository agentPluginRepository)
    {
        _agentRepository = agentRepository;
        _pluginRepository = pluginRepository;
        _agentPluginRepository = agentPluginRepository;
    }

    public async Task<bool> Handle(UpdateAgentPluginCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // 驗證 Plugin 存在
        var plugin = await _pluginRepository.GetByIdAsync(request.PluginId, cancellationToken)
            ?? throw new KeyNotFoundException($"Plugin with ID {request.PluginId} not found");

        // 查詢 AgentPlugin 關聯
        var agentPlugin = await _agentPluginRepository.GetByAgentAndPluginAsync(
            request.AgentId,
            request.PluginId,
            cancellationToken);

        if (agentPlugin == null)
        {
            throw new InvalidOperationException($"Plugin {plugin.Name} is not added to this agent");
        }

        // 更新配置
        agentPlugin.UpdateConfiguration(
            isEnabled: request.IsEnabled,
            executionOrder: request.ExecutionOrder,
            customConfiguration: request.CustomConfiguration);

        // 儲存到資料庫
        await _agentPluginRepository.UpdateAsync(agentPlugin, cancellationToken);

        return true;
    }
}
