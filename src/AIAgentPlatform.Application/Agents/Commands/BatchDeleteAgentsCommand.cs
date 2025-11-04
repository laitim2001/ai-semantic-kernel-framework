using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to batch delete (soft delete/archive) multiple agents
/// </summary>
public sealed record BatchDeleteAgentsCommand : IRequest<BatchOperationResult>
{
    /// <summary>
    /// List of agent IDs to delete (soft delete)
    /// </summary>
    public List<Guid> AgentIds { get; init; } = new();
}
