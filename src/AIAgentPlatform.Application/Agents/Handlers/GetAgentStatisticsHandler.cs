using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent 執行統計資訊查詢
/// </summary>
public class GetAgentStatisticsHandler : IRequestHandler<GetAgentStatistics, AgentStatisticsDto>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentExecutionRepository _executionRepository;

    public GetAgentStatisticsHandler(
        IAgentRepository agentRepository,
        IAgentExecutionRepository executionRepository)
    {
        _agentRepository = agentRepository;
        _executionRepository = executionRepository;
    }

    public async Task<AgentStatisticsDto> Handle(GetAgentStatistics request, CancellationToken cancellationToken)
    {
        // 驗證 Agent 存在
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new EntityNotFoundException($"Agent with ID {request.AgentId} not found");

        // 使用提供的日期範圍，或預設為最近一個月
        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        // 驗證日期範圍有效性
        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be greater than or equal to start date");
        }

        // 從 AgentExecution Repository 獲取統計資訊
        var (totalExecutions, successfulExecutions, failedExecutions, avgResponseTimeMs) =
            await _executionRepository.GetStatisticsAsync(
                request.AgentId,
                startDate,
                endDate,
                cancellationToken);

        var successRate = totalExecutions > 0
            ? Math.Round((decimal)successfulExecutions / totalExecutions * 100, 2)
            : 0;

        // 獲取最後一次執行時間
        var (recentExecutions, _) = await _executionRepository.GetByAgentIdAsync(
            request.AgentId,
            startDate,
            endDate,
            skip: 0,
            take: 1,
            cancellationToken: cancellationToken);

        var lastExecutionTime = recentExecutions.FirstOrDefault()?.StartTime;

        return new AgentStatisticsDto
        {
            AgentId = agent.Id,
            Name = agent.Name,
            TotalExecutions = totalExecutions,
            SuccessfulExecutions = successfulExecutions,
            FailedExecutions = failedExecutions,
            SuccessRate = successRate,
            AverageResponseTimeMs = avgResponseTimeMs,
            LastExecutionTime = lastExecutionTime,
            PeriodStart = startDate,
            PeriodEnd = endDate
        };
    }
}
