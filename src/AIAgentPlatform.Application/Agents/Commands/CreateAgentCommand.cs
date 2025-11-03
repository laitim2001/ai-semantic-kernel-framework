using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to create a new agent
/// </summary>
public sealed record CreateAgentCommand : IRequest<AgentDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-4o";
    public decimal Temperature { get; init; } = 0.7m;
    public int MaxTokens { get; init; } = 4096;
}
