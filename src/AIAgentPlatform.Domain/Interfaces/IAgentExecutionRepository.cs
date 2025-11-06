using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// Repository interface for AgentExecution entity operations
/// </summary>
public interface IAgentExecutionRepository
{
    /// <summary>
    /// Gets an execution by ID
    /// </summary>
    Task<AgentExecution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all executions for a specific agent
    /// </summary>
    Task<List<AgentExecution>> GetByAgentIdAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all executions for a specific conversation
    /// </summary>
    Task<List<AgentExecution>> GetByConversationIdAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets execution statistics for an agent
    /// </summary>
    Task<(int Total, int Successful, int Failed, double AvgResponseTimeMs)> GetStatisticsAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new execution record
    /// </summary>
    Task<AgentExecution> AddAsync(AgentExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing execution record
    /// </summary>
    Task UpdateAsync(AgentExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an execution record
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
