using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to update an existing agent
/// </summary>
public sealed record UpdateAgentCommand : IRequest<AgentDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-4o";
    public decimal Temperature { get; init; } = 0.7m;
    public int MaxTokens { get; init; } = 4096;
}
