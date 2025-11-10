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
    /// Gets a plugin by unique plugin ID
    /// </summary>
    Task<Plugin?> GetByPluginIdAsync(string pluginId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all plugins belonging to a specific user
    /// </summary>
    Task<List<Plugin>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all plugins with optional filtering, searching, and sorting
    /// </summary>
    Task<List<Plugin>> GetAllAsync(
        Guid? userId = null,
        string? status = null,
        string? category = null,
        string? searchTerm = null,
        string? sortBy = null,
        string? sortOrder = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of plugins with optional filtering and searching
    /// </summary>
    Task<int> GetCountAsync(
        Guid? userId = null,
        string? status = null,
        string? category = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches plugins by name or description
    /// </summary>
    Task<List<Plugin>> SearchAsync(
        string searchTerm,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets plugins by category
    /// </summary>
    Task<List<Plugin>> GetByCategoryAsync(
        string category,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets plugins by status
    /// </summary>
    Task<List<Plugin>> GetByStatusAsync(
        string status,
        Guid? userId = null,
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
    /// Deletes a plugin (hard delete)
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a plugin with the given ID exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a plugin with the given plugin ID exists
    /// </summary>
    Task<bool> ExistsByPluginIdAsync(string pluginId, CancellationToken cancellationToken = default);
}
