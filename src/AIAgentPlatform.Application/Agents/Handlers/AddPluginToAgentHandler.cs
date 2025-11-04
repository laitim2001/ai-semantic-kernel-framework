using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理將 Plugin 添加到 Agent
/// NOTE: 目前為骨架實作,實際的 Plugin 實體和關聯邏輯需在後續階段完善
/// </summary>
public class AddPluginToAgentHandler : IRequestHandler<AddPluginToAgentCommand, bool>
{
    private readonly IAgentRepository _agentRepository;

    public AddPluginToAgentHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<bool> Handle(AddPluginToAgentCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // TODO: 實作實際的 Plugin 添加邏輯
        // 需要:
        // 1. 驗證 Plugin 存在
        // 2. 檢查是否已經添加過
        // 3. 建立 AgentPlugin 關聯
        // 4. 儲存到資料庫
        // 目前返回 false 作為骨架實作
        return await Task.FromResult(false);
    }
}
