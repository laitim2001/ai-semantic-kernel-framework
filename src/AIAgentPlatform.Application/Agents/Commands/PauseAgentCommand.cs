using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to pause an agent
/// </summary>
public sealed record PauseAgentCommand : IRequest<AgentDto>
{
    public Guid Id { get; init; }
}
