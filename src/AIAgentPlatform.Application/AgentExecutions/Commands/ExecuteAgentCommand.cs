using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Commands;

/// <summary>
/// Command to execute an agent with a given input
/// </summary>
public sealed record ExecuteAgentCommand : IRequest<AgentExecutionResultDto>
{
    public Guid AgentId { get; init; }
    public Guid ConversationId { get; init; }
    public string Input { get; init; } = string.Empty;
    public string? Metadata { get; init; }
}
