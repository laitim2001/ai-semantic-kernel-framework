using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Application.AgentExecutions.Services;

/// <summary>
/// Service interface for executing agents
/// </summary>
public interface IAgentExecutionService
{
    /// <summary>
    /// Executes an agent with the given input
    /// </summary>
    /// <param name="agent">The agent to execute</param>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="input">The input prompt</param>
    /// <param name="metadata">Optional metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Execution result with output and metrics</returns>
    Task<(AgentExecution Execution, string Output)> ExecuteAsync(
        Agent agent,
        Guid conversationId,
        string input,
        string? metadata = null,
        CancellationToken cancellationToken = default);
}
