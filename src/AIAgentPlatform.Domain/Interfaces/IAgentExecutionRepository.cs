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
    /// Gets execution by ID with navigation properties loaded
    /// </summary>
    Task<AgentExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all executions for a specific agent with advanced filtering
    /// </summary>
    Task<(List<AgentExecution> Items, int TotalCount)> GetByAgentIdAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        Guid? conversationId = null,
        int? minTokens = null,
        int? maxTokens = null,
        double? minResponseTimeMs = null,
        double? maxResponseTimeMs = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
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
