using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// Repository interface for Agent entity operations
/// </summary>
public interface IAgentRepository
{
    /// <summary>
    /// Gets an agent by ID
    /// </summary>
    Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all agents belonging to a specific user
    /// </summary>
    Task<List<Agent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all agents with optional filtering
    /// </summary>
    Task<List<Agent>> GetAllAsync(
        Guid? userId = null,
        string? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of agents with optional filtering
    /// </summary>
    Task<int> GetCountAsync(
        Guid? userId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches agents by name or description
    /// </summary>
    Task<List<Agent>> SearchAsync(
        string searchTerm,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new agent
    /// </summary>
    Task<Agent> AddAsync(Agent agent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing agent
    /// </summary>
    Task UpdateAsync(Agent agent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an agent (hard delete)
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an agent with the given ID exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
