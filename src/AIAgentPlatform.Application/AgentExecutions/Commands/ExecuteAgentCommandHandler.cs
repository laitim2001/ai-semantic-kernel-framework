using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Application.AgentExecutions.Services;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Commands;

/// <summary>
/// Handler for ExecuteAgentCommand
/// </summary>
public sealed class ExecuteAgentCommandHandler : IRequestHandler<ExecuteAgentCommand, AgentExecutionResultDto>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAgentExecutionService _executionService;

    public ExecuteAgentCommandHandler(
        IAgentRepository agentRepository,
        IAgentExecutionService executionService)
    {
        _agentRepository = agentRepository;
        _executionService = executionService;
    }

    public async Task<AgentExecutionResultDto> Handle(ExecuteAgentCommand request, CancellationToken cancellationToken)
    {

        // Get agent
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken);
        if (agent == null)
        {
            throw new InvalidOperationException($"Agent {request.AgentId} not found");
        }

        // Validate agent is active
        if (!agent.Status.IsActive)
        {
            throw new InvalidOperationException($"Agent {request.AgentId} is not active (status: {agent.Status.Value})");
        }

        // Execute agent
        var (execution, output) = await _executionService.ExecuteAsync(
            agent,
            request.ConversationId,
            request.Input,
            request.Metadata,
            cancellationToken);

        // Map to DTO
        return new AgentExecutionResultDto
        {
            ExecutionId = execution.Id,
            AgentId = execution.AgentId,
            ConversationId = execution.ConversationId,
            Status = execution.Status.Value,
            Output = output,
            ErrorMessage = execution.ErrorMessage,
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            ResponseTimeMs = execution.ResponseTimeMs,
            TokensUsed = execution.TokensUsed
        };
    }
}
