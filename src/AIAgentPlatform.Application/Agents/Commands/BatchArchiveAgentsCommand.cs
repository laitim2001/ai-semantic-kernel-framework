using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to batch archive multiple agents
/// </summary>
public sealed record BatchArchiveAgentsCommand : IRequest<BatchOperationResult>
{
    /// <summary>
    /// List of agent IDs to archive
    /// </summary>
    public List<Guid> AgentIds { get; init; } = new();
}
