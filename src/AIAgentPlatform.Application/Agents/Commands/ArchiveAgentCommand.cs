using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to archive an agent (soft delete alternative)
/// </summary>
public sealed record ArchiveAgentCommand : IRequest<AgentDto>
{
    public Guid Id { get; init; }
}
