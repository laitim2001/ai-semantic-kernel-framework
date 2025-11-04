using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent Plugins 查詢
/// NOTE: 目前為骨架實作,實際的 Plugin 實體和資料庫結構需在後續階段完善
/// </summary>
public class GetAgentPluginsHandler : IRequestHandler<GetAgentPlugins, List<AgentPluginDto>>
{
    private readonly IAgentRepository _agentRepository;

    public GetAgentPluginsHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<List<AgentPluginDto>> Handle(GetAgentPlugins request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        // TODO: 實作實際的 Plugin 查詢邏輯
        // 需要新增 Plugin 和 AgentPlugin 實體及對應的 DbSet
        // 目前返回空列表作為骨架實作
        return new List<AgentPluginDto>();
    }
}
