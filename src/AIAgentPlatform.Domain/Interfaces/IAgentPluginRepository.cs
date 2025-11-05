using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// Repository interface for AgentPlugin entity operations
/// </summary>
public interface IAgentPluginRepository
{
    /// <summary>
    /// Gets an agent-plugin association by ID
    /// </summary>
    Task<AgentPlugin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an agent-plugin association by agent and plugin IDs
    /// </summary>
    Task<AgentPlugin?> GetByAgentAndPluginAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all plugins for a specific agent
    /// </summary>
    Task<List<AgentPlugin>> GetByAgentIdAsync(
        Guid agentId,
        bool? enabledOnly = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all agents using a specific plugin
    /// </summary>
    Task<List<AgentPlugin>> GetByPluginIdAsync(
        Guid pluginId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new agent-plugin association
    /// </summary>
    Task<AgentPlugin> AddAsync(AgentPlugin agentPlugin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing agent-plugin association
    /// </summary>
    Task UpdateAsync(AgentPlugin agentPlugin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an agent-plugin association
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an agent-plugin association by agent and plugin IDs
    /// </summary>
    Task DeleteByAgentAndPluginAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an agent-plugin association exists
    /// </summary>
    Task<bool> ExistsAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default);
}
