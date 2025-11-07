using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetAgentExecutionStatistics query
/// </summary>
public sealed class GetAgentExecutionStatisticsHandler
    : IRequestHandler<GetAgentExecutionStatistics, AgentExecutionStatisticsDto>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetAgentExecutionStatisticsHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<AgentExecutionStatisticsDto> Handle(
        GetAgentExecutionStatistics request,
        CancellationToken cancellationToken)
    {
        // Get basic statistics
        var (total, successful, failed, cancelled, avgResponseTime) = await _executionRepository.GetStatisticsAsync(
            request.AgentId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        // Get detailed metrics
        var (minResponseTime, maxResponseTime, medianResponseTime, p95ResponseTime, p99ResponseTime,
            totalTokens, avgTokens, minTokens, maxTokens) = await _executionRepository.GetDetailedMetricsAsync(
            request.AgentId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var successRate = total > 0 ? (double)successful / total * 100 : 0;

        return new AgentExecutionStatisticsDto
        {
            AgentId = request.AgentId,
            TotalExecutions = total,
            SuccessfulExecutions = successful,
            FailedExecutions = failed,
            CancelledExecutions = cancelled,
            SuccessRate = Math.Round(successRate, 2),
            AverageResponseTimeMs = Math.Round(avgResponseTime, 2),
            MinResponseTimeMs = minResponseTime.HasValue ? Math.Round(minResponseTime.Value, 2) : null,
            MaxResponseTimeMs = maxResponseTime.HasValue ? Math.Round(maxResponseTime.Value, 2) : null,
            MedianResponseTimeMs = medianResponseTime.HasValue ? Math.Round(medianResponseTime.Value, 2) : null,
            P95ResponseTimeMs = p95ResponseTime.HasValue ? Math.Round(p95ResponseTime.Value, 2) : null,
            P99ResponseTimeMs = p99ResponseTime.HasValue ? Math.Round(p99ResponseTime.Value, 2) : null,
            TotalTokensUsed = totalTokens,
            AverageTokensPerExecution = Math.Round(avgTokens, 2),
            MinTokensUsed = minTokens,
            MaxTokensUsed = maxTokens,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }
}
