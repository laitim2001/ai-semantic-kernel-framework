using AIAgentPlatform.Application.AgentExecutions.DTOs;

namespace AIAgentPlatform.Application.AgentExecutions.Services;

/// <summary>
/// Service for exporting agent execution data in various formats
/// </summary>
public interface IExecutionExportService
{
    /// <summary>
    /// Export execution history to CSV format
    /// </summary>
    Task<byte[]> ExportToCsvAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export execution history to JSON format
    /// </summary>
    Task<string> ExportToJsonAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export execution statistics to CSV format
    /// </summary>
    Task<byte[]> ExportStatisticsToCsvAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export execution statistics to JSON format
    /// </summary>
    Task<string> ExportStatisticsToJsonAsync(
        Guid agentId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
