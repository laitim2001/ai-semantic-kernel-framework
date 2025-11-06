using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get detailed execution information including navigation properties
/// </summary>
public sealed record GetExecutionDetails : IRequest<AgentExecutionDetailDto?>
{
    public Guid ExecutionId { get; init; }
}
