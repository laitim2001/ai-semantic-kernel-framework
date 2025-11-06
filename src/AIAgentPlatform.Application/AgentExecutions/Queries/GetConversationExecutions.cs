using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get all executions for a specific conversation
/// </summary>
public sealed record GetConversationExecutions : IRequest<List<AgentExecutionDto>>
{
    public Guid ConversationId { get; init; }
}
