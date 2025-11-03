using AIAgentPlatform.Application.Agents.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// Query to get a single agent by ID
/// </summary>
public sealed record GetAgentByIdQuery : IRequest<AgentDto?>
{
    public Guid Id { get; init; }
}
