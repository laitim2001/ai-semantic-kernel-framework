using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// 查詢 Agent 執行統計資訊
/// </summary>
public record GetAgentStatistics(
    Guid AgentId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<AgentStatisticsDto>;
