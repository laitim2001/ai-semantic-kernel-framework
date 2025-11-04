using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理從 Agent 移除 Plugin
/// NOTE: 目前為骨架實作,實際的 Plugin 移除邏輯需在後續階段完善
/// </summary>
public class RemovePluginFromAgentHandler : IRequestHandler<RemovePluginFromAgentCommand, bool>
{
    private readonly IAgentRepository _agentRepository;

    public RemovePluginFromAgentHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<bool> Handle(RemovePluginFromAgentCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // TODO: 實作實際的 Plugin 移除邏輯
        // 需要:
        // 1. 查詢 AgentPlugin 關聯
        // 2. 刪除關聯記錄
        // 3. 更新資料庫
        // 目前返回 false 作為骨架實作
        return await Task.FromResult(false);
    }
}
