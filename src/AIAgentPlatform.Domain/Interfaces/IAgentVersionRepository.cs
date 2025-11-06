using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// Repository interface for AgentVersion entity operations
/// </summary>
public interface IAgentVersionRepository
{
    /// <summary>
    /// Gets a version by ID
    /// </summary>
    Task<AgentVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all versions for a specific agent
    /// </summary>
    Task<List<AgentVersion>> GetByAgentIdAsync(
        Guid agentId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current version for a specific agent
    /// </summary>
    Task<AgentVersion?> GetCurrentVersionAsync(
        Guid agentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific version by agent ID and version number
    /// </summary>
    Task<AgentVersion?> GetByVersionNumberAsync(
        Guid agentId,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets version count for a specific agent
    /// </summary>
    Task<int> GetCountByAgentIdAsync(
        Guid agentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new version
    /// </summary>
    Task<AgentVersion> AddAsync(AgentVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing version
    /// </summary>
    Task UpdateAsync(AgentVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a version
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all versions for an agent as not current
    /// </summary>
    Task MarkAllAsNotCurrentAsync(Guid agentId, CancellationToken cancellationToken = default);
}
