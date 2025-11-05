using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAgentPlatform.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for AgentPlugin entity
/// </summary>
public sealed class AgentPluginRepository : IAgentPluginRepository
{
    private readonly ApplicationDbContext _context;

    public AgentPluginRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AgentPlugin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgentPlugins
            .Include(ap => ap.Agent)
            .Include(ap => ap.Plugin)
            .FirstOrDefaultAsync(ap => ap.Id == id, cancellationToken);
    }

    public async Task<AgentPlugin?> GetByAgentAndPluginAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentPlugins
            .Include(ap => ap.Agent)
            .Include(ap => ap.Plugin)
            .FirstOrDefaultAsync(ap => ap.AgentId == agentId && ap.PluginId == pluginId, cancellationToken);
    }

    public async Task<List<AgentPlugin>> GetByAgentIdAsync(
        Guid agentId,
        bool? enabledOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AgentPlugins
            .Include(ap => ap.Plugin)
            .Where(ap => ap.AgentId == agentId);

        if (enabledOnly.HasValue && enabledOnly.Value)
        {
            query = query.Where(ap => ap.IsEnabled);
        }

        return await query
            .OrderBy(ap => ap.ExecutionOrder)
            .ThenBy(ap => ap.Plugin!.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AgentPlugin>> GetByPluginIdAsync(
        Guid pluginId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentPlugins
            .Include(ap => ap.Agent)
            .Where(ap => ap.PluginId == pluginId)
            .OrderBy(ap => ap.Agent!.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AgentPlugin> AddAsync(AgentPlugin agentPlugin, CancellationToken cancellationToken = default)
    {
        await _context.AgentPlugins.AddAsync(agentPlugin, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return agentPlugin;
    }

    public async Task UpdateAsync(AgentPlugin agentPlugin, CancellationToken cancellationToken = default)
    {
        _context.AgentPlugins.Update(agentPlugin);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agentPlugin = await GetByIdAsync(id, cancellationToken);
        if (agentPlugin != null)
        {
            _context.AgentPlugins.Remove(agentPlugin);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteByAgentAndPluginAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default)
    {
        var agentPlugin = await GetByAgentAndPluginAsync(agentId, pluginId, cancellationToken);
        if (agentPlugin != null)
        {
            _context.AgentPlugins.Remove(agentPlugin);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(
        Guid agentId,
        Guid pluginId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentPlugins.AnyAsync(
            ap => ap.AgentId == agentId && ap.PluginId == pluginId,
            cancellationToken);
    }
}
