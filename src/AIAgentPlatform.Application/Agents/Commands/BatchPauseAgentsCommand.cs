using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to batch pause multiple agents
/// </summary>
public sealed record BatchPauseAgentsCommand : IRequest<BatchOperationResult>
{
    /// <summary>
    /// List of agent IDs to pause
    /// </summary>
    public List<Guid> AgentIds { get; init; } = new();
}
