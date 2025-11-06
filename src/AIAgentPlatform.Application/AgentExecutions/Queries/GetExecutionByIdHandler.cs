using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetExecutionById query
/// </summary>
public sealed class GetExecutionByIdHandler : IRequestHandler<GetExecutionById, AgentExecutionDto?>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetExecutionByIdHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<AgentExecutionDto?> Handle(GetExecutionById request, CancellationToken cancellationToken)
    {
        var execution = await _executionRepository.GetByIdAsync(request.ExecutionId, cancellationToken);

        return execution == null ? null : MapToDto(execution);
    }

    private static AgentExecutionDto MapToDto(Domain.Entities.AgentExecution execution)
    {
        return new AgentExecutionDto
        {
            Id = execution.Id,
            AgentId = execution.AgentId,
            ConversationId = execution.ConversationId,
            Status = execution.Status.Value,
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            ResponseTimeMs = execution.ResponseTimeMs,
            TokensUsed = execution.TokensUsed,
            ErrorMessage = execution.ErrorMessage,
            Metadata = execution.Metadata
        };
    }
}
