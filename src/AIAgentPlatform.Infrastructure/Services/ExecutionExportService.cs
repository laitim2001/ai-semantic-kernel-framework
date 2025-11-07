using System.Globalization;
using System.Text;
using System.Text.Json;
using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Application.AgentExecutions.Services;
using AIAgentPlatform.Domain.Interfaces;

namespace AIAgentPlatform.Infrastructure.Services;

/// <summary>
/// Implementation of execution export service
/// </summary>
public sealed class ExecutionExportService : IExecutionExportService
{
    private readonly IAgentExecutionRepository _executionRepository;

    public ExecutionExportService(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<byte[]> ExportToCsvAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var (executions, _) = await _executionRepository.GetByAgentIdAsync(
            agentId,
            startDate,
            endDate,
            status: null,
            conversationId: null,
            minTokens: null,
            maxTokens: null,
            minResponseTimeMs: null,
            maxResponseTimeMs: null,
            searchTerm: null,
            sortBy: null,
            sortDescending: true,
            skip: 0,
            take: int.MaxValue,  // Get all records for export
            cancellationToken);

        var csv = new StringBuilder();

        // Header
        csv.AppendLine("ExecutionId,AgentId,ConversationId,Status,StartTime,EndTime,ResponseTimeMs,TokensUsed,ErrorMessage");

        // Data rows
        foreach (var execution in executions.OrderBy(e => e.StartTime))
        {
            csv.AppendLine(string.Join(",",
                execution.Id,
                execution.AgentId,
                execution.ConversationId,
                EscapeCsvField(execution.Status.Value),
                execution.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                execution.EndTime?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture) ?? "",
                execution.ResponseTimeMs?.ToString(CultureInfo.InvariantCulture) ?? "",
                execution.TokensUsed?.ToString(CultureInfo.InvariantCulture) ?? "",
                EscapeCsvField(execution.ErrorMessage ?? "")
            ));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<string> ExportToJsonAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var (executions, _) = await _executionRepository.GetByAgentIdAsync(
            agentId,
            startDate,
            endDate,
            status: null,
            conversationId: null,
            minTokens: null,
            maxTokens: null,
            minResponseTimeMs: null,
            maxResponseTimeMs: null,
            searchTerm: null,
            sortBy: null,
            sortDescending: true,
            skip: 0,
            take: int.MaxValue,  // Get all records for export
            cancellationToken);

        var exportData = executions.OrderBy(e => e.StartTime).Select(e => new
        {
            ExecutionId = e.Id,
            AgentId = e.AgentId,
            ConversationId = e.ConversationId,
            Status = e.Status.Value,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            ResponseTimeMs = e.ResponseTimeMs,
            TokensUsed = e.TokensUsed,
            ErrorMessage = e.ErrorMessage
        });

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(new { executions = exportData }, options);
    }

    public async Task<byte[]> ExportStatisticsToCsvAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var (total, successful, failed, cancelled, avgResponseTime) =
            await _executionRepository.GetStatisticsAsync(agentId, startDate, endDate, cancellationToken);

        var (minResponseTime, maxResponseTime, medianResponseTime, p95ResponseTime, p99ResponseTime,
             totalTokens, avgTokens, minTokens, maxTokens) =
            await _executionRepository.GetDetailedMetricsAsync(agentId, startDate, endDate, cancellationToken);

        var successRate = total > 0 ? (double)successful / total * 100 : 0;

        var csv = new StringBuilder();

        // Statistics section
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Agent ID,{agentId}");
        csv.AppendLine($"Start Date,{startDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture) ?? "All time"}");
        csv.AppendLine($"End Date,{endDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture) ?? "Present"}");
        csv.AppendLine($"Total Executions,{total}");
        csv.AppendLine($"Successful Executions,{successful}");
        csv.AppendLine($"Failed Executions,{failed}");
        csv.AppendLine($"Cancelled Executions,{cancelled}");
        csv.AppendLine($"Success Rate (%),{successRate:F2}");
        csv.AppendLine();

        csv.AppendLine("Response Time Metrics,Value (ms)");
        csv.AppendLine($"Average Response Time,{avgResponseTime:F2}");
        csv.AppendLine($"Min Response Time,{minResponseTime?.ToString("F2", CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine($"Max Response Time,{maxResponseTime?.ToString("F2", CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine($"Median Response Time (P50),{medianResponseTime?.ToString("F2", CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine($"95th Percentile (P95),{p95ResponseTime?.ToString("F2", CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine($"99th Percentile (P99),{p99ResponseTime?.ToString("F2", CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine();

        csv.AppendLine("Token Usage Metrics,Value");
        csv.AppendLine($"Total Tokens Used,{totalTokens}");
        csv.AppendLine($"Average Tokens Per Execution,{avgTokens:F2}");
        csv.AppendLine($"Min Tokens Used,{minTokens?.ToString(CultureInfo.InvariantCulture) ?? "N/A"}");
        csv.AppendLine($"Max Tokens Used,{maxTokens?.ToString(CultureInfo.InvariantCulture) ?? "N/A"}");

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<string> ExportStatisticsToJsonAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var (total, successful, failed, cancelled, avgResponseTime) =
            await _executionRepository.GetStatisticsAsync(agentId, startDate, endDate, cancellationToken);

        var (minResponseTime, maxResponseTime, medianResponseTime, p95ResponseTime, p99ResponseTime,
             totalTokens, avgTokens, minTokens, maxTokens) =
            await _executionRepository.GetDetailedMetricsAsync(agentId, startDate, endDate, cancellationToken);

        var successRate = total > 0 ? (double)successful / total * 100 : 0;

        var statistics = new
        {
            agentId,
            period = new
            {
                startDate,
                endDate
            },
            executionCounts = new
            {
                total,
                successful,
                failed,
                cancelled,
                successRate
            },
            responseTimeMetrics = new
            {
                averageMs = avgResponseTime,
                minMs = minResponseTime,
                maxMs = maxResponseTime,
                medianMs = medianResponseTime,
                p95Ms = p95ResponseTime,
                p99Ms = p99ResponseTime
            },
            tokenUsageMetrics = new
            {
                totalTokens,
                averageTokensPerExecution = avgTokens,
                minTokens,
                maxTokens
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(statistics, options);
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return field;

        // If field contains comma, newline, or quotes, wrap in quotes and escape quotes
        if (field.Contains(',') || field.Contains('\n') || field.Contains('"'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
