using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to batch activate multiple agents
/// </summary>
public sealed record BatchActivateAgentsCommand : IRequest<BatchOperationResult>
{
    /// <summary>
    /// List of agent IDs to activate
    /// </summary>
    public List<Guid> AgentIds { get; init; } = new();
}
