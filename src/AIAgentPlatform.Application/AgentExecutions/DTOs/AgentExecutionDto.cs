namespace AIAgentPlatform.Application.AgentExecutions.DTOs;

/// <summary>
/// DTO for agent execution record
/// </summary>
public sealed class AgentExecutionDto
{
    public Guid Id { get; init; }
    public Guid AgentId { get; init; }
    public Guid ConversationId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public double? ResponseTimeMs { get; init; }
    public int? TokensUsed { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Metadata { get; init; }
}
