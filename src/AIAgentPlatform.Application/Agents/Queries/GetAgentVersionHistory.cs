using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// 查詢 Agent 版本歷史記錄
/// </summary>
public record GetAgentVersionHistory(
    Guid AgentId,
    int Skip = 0,
    int Take = 20
) : IRequest<List<AgentVersionDto>>;
