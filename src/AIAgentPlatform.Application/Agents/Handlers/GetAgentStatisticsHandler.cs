using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Handlers;

/// <summary>
/// 處理 Agent 執行統計查詢
/// </summary>
public class GetAgentStatisticsHandler : IRequestHandler<GetAgentStatistics, AgentStatisticsDto>
{
    private readonly IAgentRepository _agentRepository;

    public GetAgentStatisticsHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<AgentStatisticsDto> Handle(GetAgentStatistics request, CancellationToken cancellationToken)
    {
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Agent with ID {request.AgentId} not found");

        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        // TODO: 實際的統計計算需要從 Conversation 和 Message 資料庫查詢
        // 目前返回模擬數據作為骨架實作
        var conversations = new List<AIAgentPlatform.Domain.Entities.Conversation>();

        var totalExecutions = conversations.Count;

        // 假設: Conversation 有至少一個 Message 且沒有錯誤訊息就算成功
        var successfulExecutions = conversations
            .Count(c => c.Messages.Any() && !c.Messages.Any(m => m.Content.Contains("error", StringComparison.OrdinalIgnoreCase)));

        var failedExecutions = totalExecutions - successfulExecutions;

        var successRate = totalExecutions > 0
            ? Math.Round((decimal)successfulExecutions / totalExecutions * 100, 2)
            : 0;

        // 計算平均響應時間 (基於 Conversation 的第一個和最後一個 Message 的時間差)
        var responseTimes = conversations
            .Where(c => c.Messages.Count >= 2)
            .Select(c =>
            {
                var messages = c.Messages.OrderBy(m => m.CreatedAt).ToList();
                return (messages.Last().CreatedAt - messages.First().CreatedAt).TotalMilliseconds;
            })
            .ToList();

        var averageResponseTimeMs = responseTimes.Any()
            ? Math.Round(responseTimes.Average(), 2)
            : 0;

        var lastExecutionTime = conversations.Any()
            ? conversations.Max(c => c.CreatedAt)
            : (DateTime?)null;

        return new AgentStatisticsDto
        {
            AgentId = agent.Id,
            Name = agent.Name,
            TotalExecutions = totalExecutions,
            SuccessfulExecutions = successfulExecutions,
            FailedExecutions = failedExecutions,
            SuccessRate = successRate,
            AverageResponseTimeMs = averageResponseTimeMs,
            LastExecutionTime = lastExecutionTime,
            PeriodStart = startDate,
            PeriodEnd = endDate
        };
    }
}
