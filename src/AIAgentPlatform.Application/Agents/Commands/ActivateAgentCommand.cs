using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to activate an agent
/// </summary>
public sealed record ActivateAgentCommand : IRequest<AgentDto>
{
    public Guid Id { get; init; }
}
