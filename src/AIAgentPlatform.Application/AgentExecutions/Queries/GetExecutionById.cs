using AIAgentPlatform.Application.AgentExecutions.DTOs;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Query to get a single execution by ID
/// </summary>
public sealed record GetExecutionById(Guid ExecutionId) : IRequest<AgentExecutionDto?>;
