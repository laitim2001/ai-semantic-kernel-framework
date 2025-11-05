using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理將 Plugin 添加到 Agent
/// </summary>
public class AddPluginToAgentHandler : IRequestHandler<AddPluginToAgentCommand, bool>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly IAgentPluginRepository _agentPluginRepository;

    public AddPluginToAgentHandler(
        IAgentRepository agentRepository,
        IPluginRepository pluginRepository,
        IAgentPluginRepository agentPluginRepository)
    {
        _agentRepository = agentRepository;
        _pluginRepository = pluginRepository;
        _agentPluginRepository = agentPluginRepository;
    }

    public async Task<bool> Handle(AddPluginToAgentCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // 驗證 Plugin 存在
        var plugin = await _pluginRepository.GetByIdAsync(request.PluginId, cancellationToken)
            ?? throw new KeyNotFoundException($"Plugin with ID {request.PluginId} not found");

        // 檢查是否已經添加過
        var exists = await _agentPluginRepository.ExistsAsync(
            request.AgentId,
            request.PluginId,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Plugin {plugin.Name} is already added to this agent");
        }

        // 建立 AgentPlugin 關聯
        var agentPlugin = AgentPlugin.Create(
            agentId: request.AgentId,
            pluginId: request.PluginId,
            addedBy: request.UserId,
            executionOrder: request.ExecutionOrder,
            customConfiguration: request.CustomConfiguration);

        // 儲存到資料庫
        await _agentPluginRepository.AddAsync(agentPlugin, cancellationToken);

        return true;
    }
}
