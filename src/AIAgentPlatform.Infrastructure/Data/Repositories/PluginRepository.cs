using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAgentPlatform.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Plugin entity
/// </summary>
public sealed class PluginRepository : IPluginRepository
{
    private readonly ApplicationDbContext _context;

    public PluginRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Plugin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Plugin?> GetByPluginIdAsync(string pluginId, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.PluginId == pluginId, cancellationToken);
    }

    public async Task<List<Plugin>> GetAllAsync(
        Guid? userId = null,
        string? status = null,
        string? category = null,
        string? searchTerm = null,
        string? sortBy = null,
        string? sortOrder = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.AsQueryable();

        // Apply filters
        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            // Compare Status.Value with the status string
            query = query.Where(p => p.Status.Value == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category != null && p.Category.Contains(category));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.PluginId.Contains(searchTerm) ||
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        // Apply sorting
        query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("name", "desc") => query.OrderByDescending(p => p.Name),
            ("name", _) => query.OrderBy(p => p.Name),
            ("updated", "asc") => query.OrderBy(p => p.UpdatedAt),
            ("updated", _) => query.OrderByDescending(p => p.UpdatedAt),
            ("created", "asc") => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        // Apply pagination
        query = query.Skip(skip).Take(take);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Plugin> AddAsync(Plugin plugin, CancellationToken cancellationToken = default)
    {
        await _context.Plugins.AddAsync(plugin, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return plugin;
    }

    public async Task UpdateAsync(Plugin plugin, CancellationToken cancellationToken = default)
    {
        _context.Plugins.Update(plugin);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Plugin>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(
        Guid? userId = null,
        string? status = null,
        string? category = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            // Compare Status.Value with the status string
            query = query.Where(p => p.Status.Value == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category != null && p.Category.Contains(category));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.PluginId.Contains(searchTerm) ||
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Plugin>> SearchAsync(
        string searchTerm,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        query = query.Where(p =>
            p.Name.Contains(searchTerm) ||
            p.PluginId.Contains(searchTerm) ||
            (p.Description != null && p.Description.Contains(searchTerm)));

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<Plugin>> GetByCategoryAsync(
        string category,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.Where(p => p.Category != null && p.Category.Contains(category));

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<Plugin>> GetByStatusAsync(
        string status,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins
            .Where(p => p.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plugin = await GetByIdAsync(id, cancellationToken);
        if (plugin != null)
        {
            _context.Plugins.Remove(plugin);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByPluginIdAsync(string pluginId, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins.AnyAsync(p => p.PluginId == pluginId, cancellationToken);
    }
}
