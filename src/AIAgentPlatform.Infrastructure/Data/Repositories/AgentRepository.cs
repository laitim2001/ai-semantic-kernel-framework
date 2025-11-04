using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAgentPlatform.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Agent entity
/// </summary>
public sealed class AgentRepository : IAgentRepository
{
    private readonly ApplicationDbContext _context;

    public AgentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agents
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Agent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Agents
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Agent>> GetAllAsync(
        Guid? userId = null,
        string? status = null,
        string? searchTerm = null,
        string? model = null,
        string? sortBy = null,
        string? sortOrder = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Agents.AsQueryable();

        // Apply filters
        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status.Value == status.ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            query = query.Where(a => a.Model.Value == model.ToLowerInvariant());
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            query = query.Where(a =>
                a.Name.ToLower().Contains(lowerSearchTerm) ||
                (a.Description != null && a.Description.ToLower().Contains(lowerSearchTerm)) ||
                a.Id.ToString().Contains(lowerSearchTerm)
            );
        }

        // Apply sorting
        query = ApplySorting(query, sortBy, sortOrder);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Agent> ApplySorting(IQueryable<Agent> query, string? sortBy, string? sortOrder)
    {
        var isDescending = string.IsNullOrWhiteSpace(sortOrder) || sortOrder.ToLowerInvariant() == "desc";

        return (sortBy?.ToLowerInvariant()) switch
        {
            "name" => isDescending
                ? query.OrderByDescending(a => a.Name)
                : query.OrderBy(a => a.Name),
            "createdat" or "created" => isDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt),
            "updatedat" or "updated" => isDescending
                ? query.OrderByDescending(a => a.UpdatedAt)
                : query.OrderBy(a => a.UpdatedAt),
            _ => query.OrderByDescending(a => a.CreatedAt) // Default sort
        };
    }

    public async Task<int> GetCountAsync(
        Guid? userId = null,
        string? status = null,
        string? searchTerm = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Agents.AsQueryable();

        // Apply filters
        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status.Value == status.ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            query = query.Where(a => a.Model.Value == model.ToLowerInvariant());
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            query = query.Where(a =>
                a.Name.ToLower().Contains(lowerSearchTerm) ||
                (a.Description != null && a.Description.ToLower().Contains(lowerSearchTerm)) ||
                a.Id.ToString().Contains(lowerSearchTerm)
            );
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Agent>> SearchAsync(
        string searchTerm,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Agents.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        query = query.Where(a =>
            a.Name.ToLower().Contains(lowerSearchTerm) ||
            (a.Description != null && a.Description.ToLower().Contains(lowerSearchTerm))
        );

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Agent> AddAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        await _context.Agents.AddAsync(agent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return agent;
    }

    public async Task UpdateAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        _context.Agents.Update(agent);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await GetByIdAsync(id, cancellationToken);
        if (agent != null)
        {
            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agents.AnyAsync(a => a.Id == id, cancellationToken);
    }
}
