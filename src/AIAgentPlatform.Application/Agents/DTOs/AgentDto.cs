namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Data Transfer Object for Agent
/// </summary>
public sealed record AgentDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public decimal Temperature { get; init; }
    public int MaxTokens { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
