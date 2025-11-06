using System.Text.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent 版本回滾
/// </summary>
public class RollbackAgentVersionHandler : IRequestHandler<RollbackAgentVersionCommand, bool>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentVersionRepository _versionRepository;

    public RollbackAgentVersionHandler(
        IAgentRepository agentRepository,
        IAgentVersionRepository versionRepository)
    {
        _agentRepository = agentRepository;
        _versionRepository = versionRepository;
    }

    public async Task<bool> Handle(RollbackAgentVersionCommand request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new EntityNotFoundException($"Agent with ID {request.AgentId} not found");

        // 查詢目標版本
        var targetVersion = await _versionRepository.GetByIdAsync(request.VersionId, cancellationToken)
            ?? throw new EntityNotFoundException($"Version with ID {request.VersionId} not found");

        // 驗證版本屬於該 Agent
        if (targetVersion.AgentId != request.AgentId)
        {
            throw new InvalidOperationException("Version does not belong to the specified agent");
        }

        // 反序列化配置快照並恢復 Agent 配置
        var snapshot = JsonSerializer.Deserialize<AgentConfigSnapshot>(targetVersion.ConfigurationSnapshot)
            ?? throw new InvalidOperationException("Invalid configuration snapshot");

        // 更新 Agent 配置
        agent.Update(snapshot.Name, snapshot.Description, snapshot.Instructions);
        agent.UpdateModelConfiguration(
            LLMModel.From(snapshot.Model),
            snapshot.Temperature,
            snapshot.MaxTokens);

        // 根據 Status 更新 Agent 狀態
        var status = AgentStatus.From(snapshot.Status);
        if (status.IsActive && !agent.Status.IsActive)
        {
            agent.Activate();
        }
        else if (status.IsPaused && !agent.Status.IsPaused)
        {
            agent.Pause();
        }
        else if (status.IsArchived && !agent.Status.IsArchived)
        {
            agent.Archive();
        }

        await _agentRepository.UpdateAsync(agent, cancellationToken);

        // 記錄回滾資訊
        targetVersion.RecordRollback(request.UserId);

        // 標記其他版本為非當前版本
        await _versionRepository.MarkAllAsNotCurrentAsync(request.AgentId, cancellationToken);

        // 標記目標版本為當前版本
        targetVersion.MarkAsCurrent();
        await _versionRepository.UpdateAsync(targetVersion, cancellationToken);

        return true;
    }

    // 內部類別用於反序列化配置快照
    private class AgentConfigSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal Temperature { get; set; }
        public int MaxTokens { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
