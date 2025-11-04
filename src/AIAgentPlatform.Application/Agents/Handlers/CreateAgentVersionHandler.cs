using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理建立 Agent 版本
/// NOTE: 目前為骨架實作,實際的 AgentVersion 實體和資料庫結構需在後續階段完善
/// </summary>
public class CreateAgentVersionHandler : IRequestHandler<CreateAgentVersionCommand, Guid>
{
    private readonly IAgentRepository _agentRepository;

    public CreateAgentVersionHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<Guid> Handle(CreateAgentVersionCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // TODO: 實作實際的版本建立邏輯
        // 需要:
        // 1. 建立 AgentVersion 實體
        // 2. 儲存當前 Agent 配置快照
        // 3. 計算版本號
        // 4. 儲存到資料庫
        // 目前返回空 GUID 作為骨架實作
        return Guid.Empty;
    }
}
