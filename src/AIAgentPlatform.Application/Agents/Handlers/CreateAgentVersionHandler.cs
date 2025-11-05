using System.Text.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理建立 Agent 版本
/// </summary>
public class CreateAgentVersionHandler : IRequestHandler<CreateAgentVersionCommand, Guid>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentVersionRepository _versionRepository;

    public CreateAgentVersionHandler(
        IAgentRepository agentRepository,
        IAgentVersionRepository versionRepository)
    {
        _agentRepository = agentRepository;
        _versionRepository = versionRepository;
    }

    public async Task<Guid> Handle(CreateAgentVersionCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new EntityNotFoundException($"Agent with ID {request.AgentId} not found");

        // 獲取當前版本號並生成新版本
        var versionCount = await _versionRepository.GetCountByAgentIdAsync(request.AgentId, cancellationToken);
        var newVersionNumber = GenerateVersionNumber(versionCount + 1, request.ChangeType);

        // 建立配置快照 (序列化 Agent 配置)
        var configSnapshot = new
        {
            agent.Name,
            agent.Description,
            agent.Instructions,
            Model = agent.Model.Value,
            agent.Temperature,
            agent.MaxTokens,
            Status = agent.Status.Value,
            Timestamp = DateTime.UtcNow
        };
        var configurationSnapshot = JsonSerializer.Serialize(configSnapshot);

        // 建立新版本
        var version = AgentVersion.Create(
            agentId: request.AgentId,
            version: newVersionNumber,
            changeDescription: request.ChangeDescription,
            changeType: VersionChangeType.From(request.ChangeType),
            configurationSnapshot: configurationSnapshot,
            createdBy: request.UserId,
            isCurrent: true);

        // 標記其他版本為非當前版本
        await _versionRepository.MarkAllAsNotCurrentAsync(request.AgentId, cancellationToken);

        // 儲存新版本
        var savedVersion = await _versionRepository.AddAsync(version, cancellationToken);

        return savedVersion.Id;
    }

    /// <summary>
    /// 根據變更類型生成版本號 (語義化版本)
    /// </summary>
    private static string GenerateVersionNumber(int versionCount, string changeType)
    {
        // 簡化版本:第一個版本總是 v1.0.0,後續版本根據 changeType 遞增
        if (versionCount == 1)
        {
            return "v1.0.0";
        }

        // 對於後續版本,根據 changeType 決定版本號格式
        return changeType.ToLowerInvariant() switch
        {
            "major" => $"v{versionCount}.0.0",
            "minor" => $"v1.{versionCount - 1}.0",
            "patch" or "hotfix" => $"v1.0.{versionCount - 1}",
            _ => $"v{versionCount}.0.0"
        };
    }
}
