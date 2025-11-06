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
        var (total, successful, failed, avgResponseTime) = await _executionRepository.GetStatisticsAsync(
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
            SuccessRate = Math.Round(successRate, 2),
            AverageResponseTimeMs = Math.Round(avgResponseTime, 2),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }
}
