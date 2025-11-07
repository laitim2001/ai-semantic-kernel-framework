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
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<AgentExecution?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgentExecutions
            .Include(e => e.Agent)
            .Include(e => e.Conversation)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(List<AgentExecution> Items, int TotalCount)> GetByAgentIdAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        Guid? conversationId = null,
        int? minTokens = null,
        int? maxTokens = null,
        double? minResponseTimeMs = null,
        double? maxResponseTimeMs = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
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

        // Apply conversation filter
        if (conversationId.HasValue)
        {
            query = query.Where(e => e.ConversationId == conversationId.Value);
        }

        // Apply token range filter
        if (minTokens.HasValue)
        {
            query = query.Where(e => e.TokensUsed >= minTokens.Value);
        }

        if (maxTokens.HasValue)
        {
            query = query.Where(e => e.TokensUsed <= maxTokens.Value);
        }

        // Apply response time range filter
        if (minResponseTimeMs.HasValue)
        {
            query = query.Where(e => e.ResponseTimeMs >= minResponseTimeMs.Value);
        }

        if (maxResponseTimeMs.HasValue)
        {
            query = query.Where(e => e.ResponseTimeMs <= maxResponseTimeMs.Value);
        }

        // Apply search term (search in ErrorMessage and Metadata)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLowerInvariant();
            query = query.Where(e =>
                (e.ErrorMessage != null && e.ErrorMessage.ToLower().Contains(searchLower)) ||
                (e.Metadata != null && e.Metadata.ToLower().Contains(searchLower)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = (sortBy?.ToLowerInvariant()) switch
        {
            "endtime" => sortDescending
                ? query.OrderByDescending(e => e.EndTime)
                : query.OrderBy(e => e.EndTime),
            "responsetimems" or "responsetime" => sortDescending
                ? query.OrderByDescending(e => e.ResponseTimeMs)
                : query.OrderBy(e => e.ResponseTimeMs),
            "tokensused" or "tokens" => sortDescending
                ? query.OrderByDescending(e => e.TokensUsed)
                : query.OrderBy(e => e.TokensUsed),
            "status" => sortDescending
                ? query.OrderByDescending(e => e.Status.Value)
                : query.OrderBy(e => e.Status.Value),
            _ => sortDescending
                ? query.OrderByDescending(e => e.StartTime)
                : query.OrderBy(e => e.StartTime)
        };

        // Apply pagination
        var items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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

    public async Task<(int Total, int Successful, int Failed, int Cancelled, double AvgResponseTimeMs)> GetStatisticsAsync(
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
        var cancelled = executions.Count(e => e.Status.Value == "cancelled");
        var avgResponseTime = executions
            .Where(e => e.ResponseTimeMs.HasValue)
            .Select(e => e.ResponseTimeMs!.Value)
            .DefaultIfEmpty(0)
            .Average();

        return (total, successful, failed, cancelled, avgResponseTime);
    }

    public async Task<(
        double? MinResponseTimeMs,
        double? MaxResponseTimeMs,
        double? MedianResponseTimeMs,
        double? P95ResponseTimeMs,
        double? P99ResponseTimeMs,
        long TotalTokensUsed,
        double AvgTokensPerExecution,
        int? MinTokensUsed,
        int? MaxTokensUsed
    )> GetDetailedMetricsAsync(
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

        // Response Time Metrics
        var responseTimes = executions
            .Where(e => e.ResponseTimeMs.HasValue)
            .Select(e => e.ResponseTimeMs!.Value)
            .OrderBy(t => t)
            .ToList();

        double? minResponseTime = responseTimes.Any() ? responseTimes.Min() : null;
        double? maxResponseTime = responseTimes.Any() ? responseTimes.Max() : null;
        double? medianResponseTime = responseTimes.Any()
            ? CalculatePercentile(responseTimes, 0.5)
            : null;
        double? p95ResponseTime = responseTimes.Any()
            ? CalculatePercentile(responseTimes, 0.95)
            : null;
        double? p99ResponseTime = responseTimes.Any()
            ? CalculatePercentile(responseTimes, 0.99)
            : null;

        // Token Usage Metrics
        var tokensUsed = executions
            .Where(e => e.TokensUsed.HasValue)
            .Select(e => e.TokensUsed!.Value)
            .ToList();

        long totalTokens = tokensUsed.Any() ? tokensUsed.Sum() : 0;
        double avgTokens = tokensUsed.Any() ? tokensUsed.Average() : 0;
        int? minTokens = tokensUsed.Any() ? tokensUsed.Min() : null;
        int? maxTokens = tokensUsed.Any() ? tokensUsed.Max() : null;

        return (
            minResponseTime,
            maxResponseTime,
            medianResponseTime,
            p95ResponseTime,
            p99ResponseTime,
            totalTokens,
            avgTokens,
            minTokens,
            maxTokens
        );
    }

    public async Task<List<(
        DateTime Timestamp,
        int Total,
        int Successful,
        int Failed,
        double AvgResponseTimeMs,
        long TotalTokens,
        double AvgTokens
    )>> GetTimeSeriesStatisticsAsync(
        Guid agentId,
        DateTime startDate,
        DateTime endDate,
        string granularity,
        CancellationToken cancellationToken = default)
    {
        var executions = await _context.AgentExecutions
            .Where(e => e.AgentId == agentId
                && e.StartTime >= startDate
                && e.StartTime <= endDate)
            .ToListAsync(cancellationToken);

        // Group executions by time bucket
        var grouped = granularity.ToLowerInvariant() switch
        {
            "hour" => executions.GroupBy(e => new DateTime(
                e.StartTime.Year, e.StartTime.Month, e.StartTime.Day, e.StartTime.Hour, 0, 0)),
            "day" => executions.GroupBy(e => e.StartTime.Date),
            "week" => executions.GroupBy(e => GetWeekStart(e.StartTime)),
            "month" => executions.GroupBy(e => new DateTime(e.StartTime.Year, e.StartTime.Month, 1)),
            _ => throw new ArgumentException($"Invalid granularity: {granularity}")
        };

        var result = grouped
            .Select(g => {
                var total = g.Count();
                var successful = g.Count(e => e.Status.Value == "completed");
                var failed = g.Count(e => e.Status.Value == "failed");
                var avgResponseTime = g
                    .Where(e => e.ResponseTimeMs.HasValue)
                    .Select(e => e.ResponseTimeMs!.Value)
                    .DefaultIfEmpty(0)
                    .Average();
                var totalTokens = g
                    .Where(e => e.TokensUsed.HasValue)
                    .Select(e => (long)e.TokensUsed!.Value)
                    .Sum();
                var avgTokens = g
                    .Where(e => e.TokensUsed.HasValue)
                    .Select(e => (double)e.TokensUsed!.Value)
                    .DefaultIfEmpty(0)
                    .Average();

                return (
                    Timestamp: g.Key,
                    Total: total,
                    Successful: successful,
                    Failed: failed,
                    AvgResponseTimeMs: avgResponseTime,
                    TotalTokens: totalTokens,
                    AvgTokens: avgTokens
                );
            })
            .OrderBy(x => x.Timestamp)
            .ToList();

        return result;
    }

    private static double CalculatePercentile(List<double> sortedValues, double percentile)
    {
        if (!sortedValues.Any())
            return 0;

        var index = percentile * (sortedValues.Count - 1);
        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);

        if (lower == upper)
            return sortedValues[lower];

        var fraction = index - lower;
        return sortedValues[lower] * (1 - fraction) + sortedValues[upper] * fraction;
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
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
