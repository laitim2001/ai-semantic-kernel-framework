using AIAgentPlatform.Application.AgentExecutions.DTOs;

namespace AIAgentPlatform.Application.AgentExecutions.Services;

/// <summary>
/// Service for sending real-time execution notifications via SignalR
/// </summary>
public interface IExecutionNotificationService
{
    /// <summary>
    /// Notify that an execution has started
    /// </summary>
    Task NotifyExecutionStartedAsync(Guid agentId, Guid executionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify that an execution has completed successfully
    /// </summary>
    Task NotifyExecutionCompletedAsync(Guid agentId, AgentExecutionResultDto result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify that an execution has failed
    /// </summary>
    Task NotifyExecutionFailedAsync(Guid agentId, Guid executionId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify that agent statistics have been updated
    /// </summary>
    Task NotifyStatisticsUpdatedAsync(Guid agentId, AgentExecutionStatisticsDto statistics, CancellationToken cancellationToken = default);
}
