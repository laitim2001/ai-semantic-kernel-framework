using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetConversationExecutions query
/// </summary>
public sealed class GetConversationExecutionsHandler : IRequestHandler<GetConversationExecutions, List<AgentExecutionDto>>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetConversationExecutionsHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<List<AgentExecutionDto>> Handle(GetConversationExecutions request, CancellationToken cancellationToken)
    {
        var executions = await _executionRepository.GetByConversationIdAsync(
            request.ConversationId,
            cancellationToken);

        return executions.Select(MapToDto).ToList();
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
