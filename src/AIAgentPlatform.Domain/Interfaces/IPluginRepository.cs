using AIAgentPlatform.Domain.Entities;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// Repository interface for Plugin entity operations
/// </summary>
public interface IPluginRepository
{
    /// <summary>
    /// Gets a plugin by ID
    /// </summary>
    Task<Plugin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a plugin by name
    /// </summary>
    Task<Plugin?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all plugins with optional filtering
    /// </summary>
    Task<List<Plugin>> GetAllAsync(
        string? type = null,
        bool? isEnabled = null,
        string? searchTerm = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of plugins with optional filtering
    /// </summary>
    Task<int> GetCountAsync(
        string? type = null,
        bool? isEnabled = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches plugins by name or description
    /// </summary>
    Task<List<Plugin>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new plugin
    /// </summary>
    Task<Plugin> AddAsync(Plugin plugin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing plugin
    /// </summary>
    Task UpdateAsync(Plugin plugin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a plugin
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a plugin with the given ID exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a plugin with the given name exists
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
