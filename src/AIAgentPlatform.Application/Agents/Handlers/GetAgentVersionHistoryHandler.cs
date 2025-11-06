using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent 版本歷史查詢
/// </summary>
public class GetAgentVersionHistoryHandler : IRequestHandler<GetAgentVersionHistory, List<AgentVersionDto>>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentVersionRepository _versionRepository;

    public GetAgentVersionHistoryHandler(
        IAgentRepository agentRepository,
        IAgentVersionRepository versionRepository)
    {
        _agentRepository = agentRepository;
        _versionRepository = versionRepository;
    }

    public async Task<List<AgentVersionDto>> Handle(GetAgentVersionHistory request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new EntityNotFoundException($"Agent with ID {request.AgentId} not found");

        // 查詢版本歷史
        var versions = await _versionRepository.GetByAgentIdAsync(
            request.AgentId,
            request.Skip,
            request.Take,
            cancellationToken);

        // 轉換為 DTO
        return versions.Select(v => new AgentVersionDto
        {
            Id = v.Id,
            AgentId = v.AgentId,
            Version = v.Version,
            ChangeDescription = v.ChangeDescription,
            ChangeType = v.ChangeType.Value,
            ConfigurationSnapshot = v.ConfigurationSnapshot,
            CreatedBy = v.CreatedBy,
            CreatedAt = v.CreatedAt,
            IsCurrent = v.IsCurrent
        }).ToList();
    }
}
