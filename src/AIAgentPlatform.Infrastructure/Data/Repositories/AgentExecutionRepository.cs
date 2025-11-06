using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAgentPlatform.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for AgentExecution entity
/// </summary>
public sealed class AgentExecutionRepository : IAgentExecutionRepository
{
    private readonly ApplicationDbContext _context;

    public AgentExecutionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AgentExecution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgentExecutions
            .Include(e => e.Agent)
            .Include(e => e.Conversation)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<AgentExecution>> GetByAgentIdAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AgentExecutions
            .Where(e => e.AgentId == agentId);

        // Apply date filters
        if (startDate.HasValue)
        {
            query = query.Where(e => e.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.StartTime <= endDate.Value);
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(e => e.Status.Value == status.ToLowerInvariant());
        }

        return await query
            .OrderByDescending(e => e.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AgentExecution>> GetByConversationIdAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AgentExecutions
            .Where(e => e.ConversationId == conversationId)
            .OrderBy(e => e.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<(int Total, int Successful, int Failed, double AvgResponseTimeMs)> GetStatisticsAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AgentExecutions
            .Where(e => e.AgentId == agentId);

        // Apply date filters
        if (startDate.HasValue)
        {
            query = query.Where(e => e.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.StartTime <= endDate.Value);
        }

        var executions = await query.ToListAsync(cancellationToken);

        var total = executions.Count;
        var successful = executions.Count(e => e.Status.Value == "completed");
        var failed = executions.Count(e => e.Status.Value == "failed");
        var avgResponseTime = executions
            .Where(e => e.ResponseTimeMs.HasValue)
            .Select(e => e.ResponseTimeMs!.Value)
            .DefaultIfEmpty(0)
            .Average();

        return (total, successful, failed, avgResponseTime);
    }

    public async Task<AgentExecution> AddAsync(AgentExecution execution, CancellationToken cancellationToken = default)
    {
        await _context.AgentExecutions.AddAsync(execution, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return execution;
    }

    public async Task UpdateAsync(AgentExecution execution, CancellationToken cancellationToken = default)
    {
        _context.AgentExecutions.Update(execution);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var execution = await GetByIdAsync(id, cancellationToken);
        if (execution != null)
        {
            _context.AgentExecutions.Remove(execution);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
