namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// DTO for agent execution result
/// </summary>
public sealed class AgentExecutionResultDto
{
    public Guid ExecutionId { get; init; }
    public Guid AgentId { get; init; }
    public Guid ConversationId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Output { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public double? ResponseTimeMs { get; init; }
    public int? TokensUsed { get; init; }
}
