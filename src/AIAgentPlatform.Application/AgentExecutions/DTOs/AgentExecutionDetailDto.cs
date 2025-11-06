namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// Detailed execution record with navigation properties
/// </summary>
public sealed class AgentExecutionDetailDto
{
    public Guid Id { get; init; }
    public Guid AgentId { get; init; }
    public Guid ConversationId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public double? ResponseTimeMs { get; init; }
    public int? TokensUsed { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Metadata { get; init; }

    // Navigation property information
    public string? AgentName { get; init; }
    public string? ConversationTitle { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; init; }
}
