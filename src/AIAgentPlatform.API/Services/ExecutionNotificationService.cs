using AIAgentPlatform.API.Hubs;
using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Application.AgentExecutions.Services;
using Microsoft.AspNetCore.SignalR;

namespace AIAgentPlatform.API.Services;

/// <summary>
/// SignalR-based implementation of execution notification service
/// </summary>
public sealed class ExecutionNotificationService : IExecutionNotificationService
{
    private readonly IHubContext<ExecutionMonitorHub> _hubContext;
    private readonly ILogger<ExecutionNotificationService> _logger;

    public ExecutionNotificationService(
        IHubContext<ExecutionMonitorHub> hubContext,
        ILogger<ExecutionNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyExecutionStartedAsync(Guid agentId, Guid executionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                ExecutionId = executionId,
                AgentId = agentId,
                Status = "Started",
                Timestamp = DateTime.UtcNow
            };

            // Send to specific agent subscribers
            await _hubContext.Clients
                .Group($"agent-{agentId}")
                .SendAsync("ExecutionStarted", notification, cancellationToken);

            // Send to all executions subscribers
            await _hubContext.Clients
                .Group("all-executions")
                .SendAsync("ExecutionStarted", notification, cancellationToken);

            _logger.LogInformation("Execution {ExecutionId} started notification sent for agent {AgentId}",
                executionId, agentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send execution started notification for {ExecutionId}", executionId);
        }
    }

    public async Task NotifyExecutionCompletedAsync(Guid agentId, AgentExecutionResultDto result, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                ExecutionId = result.ExecutionId,
                AgentId = agentId,
                Status = "Completed",
                Result = result,
                Timestamp = DateTime.UtcNow
            };

            // Send to specific agent subscribers
            await _hubContext.Clients
                .Group($"agent-{agentId}")
                .SendAsync("ExecutionCompleted", notification, cancellationToken);

            // Send to all executions subscribers
            await _hubContext.Clients
                .Group("all-executions")
                .SendAsync("ExecutionCompleted", notification, cancellationToken);

            _logger.LogInformation("Execution {ExecutionId} completed notification sent for agent {AgentId}",
                result.ExecutionId, agentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send execution completed notification for {ExecutionId}",
                result.ExecutionId);
        }
    }

    public async Task NotifyExecutionFailedAsync(Guid agentId, Guid executionId, string errorMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                ExecutionId = executionId,
                AgentId = agentId,
                Status = "Failed",
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            };

            // Send to specific agent subscribers
            await _hubContext.Clients
                .Group($"agent-{agentId}")
                .SendAsync("ExecutionFailed", notification, cancellationToken);

            // Send to all executions subscribers
            await _hubContext.Clients
                .Group("all-executions")
                .SendAsync("ExecutionFailed", notification, cancellationToken);

            _logger.LogWarning("Execution {ExecutionId} failed notification sent for agent {AgentId}: {ErrorMessage}",
                executionId, agentId, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send execution failed notification for {ExecutionId}", executionId);
        }
    }

    public async Task NotifyStatisticsUpdatedAsync(Guid agentId, AgentExecutionStatisticsDto statistics, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                AgentId = agentId,
                Statistics = statistics,
                Timestamp = DateTime.UtcNow
            };

            // Send to specific agent subscribers
            await _hubContext.Clients
                .Group($"agent-{agentId}")
                .SendAsync("StatisticsUpdated", notification, cancellationToken);

            _logger.LogInformation("Statistics updated notification sent for agent {AgentId}", agentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send statistics updated notification for agent {AgentId}", agentId);
        }
    }
}
