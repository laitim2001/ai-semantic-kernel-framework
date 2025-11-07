using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetTimeSeriesStatistics query
/// </summary>
public sealed class GetTimeSeriesStatisticsHandler
    : IRequestHandler<GetTimeSeriesStatistics, TimeSeriesStatisticsDto>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetTimeSeriesStatisticsHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<TimeSeriesStatisticsDto> Handle(
        GetTimeSeriesStatistics request,
        CancellationToken cancellationToken)
    {
        // Validate granularity
        var validGranularities = new[] { "hour", "day", "week", "month" };
        if (!validGranularities.Contains(request.Granularity.ToLowerInvariant()))
        {
            throw new ArgumentException(
                $"Invalid granularity '{request.Granularity}'. Valid values are: {string.Join(", ", validGranularities)}");
        }

        // Get time series data
        var timeSeriesData = await _executionRepository.GetTimeSeriesStatisticsAsync(
            request.AgentId,
            request.StartDate,
            request.EndDate,
            request.Granularity,
            cancellationToken);

        // Calculate overall summary
        var totalExecutions = timeSeriesData.Sum(d => d.Total);
        var totalSuccessful = timeSeriesData.Sum(d => d.Successful);
        var overallSuccessRate = totalExecutions > 0
            ? Math.Round((double)totalSuccessful / totalExecutions * 100, 2)
            : 0;
        var overallAvgResponseTime = timeSeriesData.Any()
            ? Math.Round(timeSeriesData.Average(d => d.AvgResponseTimeMs), 2)
            : 0;
        var overallTotalTokens = timeSeriesData.Sum(d => d.TotalTokens);

        // Map to DTOs
        var dataPoints = timeSeriesData.Select(d => new ExecutionTimeSeriesDto
        {
            Timestamp = d.Timestamp,
            TotalExecutions = d.Total,
            SuccessfulExecutions = d.Successful,
            FailedExecutions = d.Failed,
            SuccessRate = d.Total > 0
                ? Math.Round((double)d.Successful / d.Total * 100, 2)
                : 0,
            AverageResponseTimeMs = Math.Round(d.AvgResponseTimeMs, 2),
            TotalTokensUsed = d.TotalTokens,
            AverageTokensPerExecution = Math.Round(d.AvgTokens, 2)
        }).ToList();

        return new TimeSeriesStatisticsDto
        {
            AgentId = request.AgentId,
            Granularity = request.Granularity.ToLowerInvariant(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DataPoints = dataPoints,
            TotalExecutions = totalExecutions,
            OverallSuccessRate = overallSuccessRate,
            OverallAverageResponseTimeMs = overallAvgResponseTime,
            OverallTotalTokensUsed = overallTotalTokens
        };
    }
}
