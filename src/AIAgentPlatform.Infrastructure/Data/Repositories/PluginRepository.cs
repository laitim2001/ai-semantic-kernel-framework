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

    public async Task<Plugin?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<List<Plugin>> GetAllAsync(
        string? type = null,
        bool? isEnabled = null,
        string? searchTerm = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(p => p.Type.Value == type.ToLowerInvariant());
        }

        if (isEnabled.HasValue)
        {
            query = query.Where(p => p.IsEnabled == isEnabled.Value);
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))
            );
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(
        string? type = null,
        bool? isEnabled = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(p => p.Type.Value == type.ToLowerInvariant());
        }

        if (isEnabled.HasValue)
        {
            query = query.Where(p => p.IsEnabled == isEnabled.Value);
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))
            );
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Plugin>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        return await _context.Plugins
            .Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))
            )
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
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

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins.AnyAsync(p => p.Name == name, cancellationToken);
    }
}
