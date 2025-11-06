using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAgentPlatform.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for AgentVersion entity
/// </summary>
public sealed class AgentVersionRepository : IAgentVersionRepository
{
    private readonly ApplicationDbContext _context;

    public AgentVersionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AgentVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgentVersions
            .Include(v => v.Agent)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<AgentVersion>> GetByAgentIdAsync(
        Guid agentId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentVersions
            .Where(v => v.AgentId == agentId)
            .OrderByDescending(v => v.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<AgentVersion?> GetCurrentVersionAsync(
        Guid agentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentVersions
            .Where(v => v.AgentId == agentId && v.IsCurrent)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AgentVersion?> GetByVersionNumberAsync(
        Guid agentId,
        string version,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentVersions
            .FirstOrDefaultAsync(v => v.AgentId == agentId && v.Version == version, cancellationToken);
    }

    public async Task<int> GetCountByAgentIdAsync(
        Guid agentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentVersions
            .CountAsync(v => v.AgentId == agentId, cancellationToken);
    }

    public async Task<AgentVersion> AddAsync(AgentVersion version, CancellationToken cancellationToken = default)
    {
        await _context.AgentVersions.AddAsync(version, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return version;
    }

    public async Task UpdateAsync(AgentVersion version, CancellationToken cancellationToken = default)
    {
        _context.AgentVersions.Update(version);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var version = await GetByIdAsync(id, cancellationToken);
        if (version != null)
        {
            _context.AgentVersions.Remove(version);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllAsNotCurrentAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var versions = await _context.AgentVersions
            .Where(v => v.AgentId == agentId && v.IsCurrent)
            .ToListAsync(cancellationToken);

        foreach (var version in versions)
        {
            version.MarkAsNotCurrent();
        }

        if (versions.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
