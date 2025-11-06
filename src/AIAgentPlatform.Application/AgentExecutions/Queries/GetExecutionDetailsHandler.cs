using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetExecutionDetails query
/// </summary>
public sealed class GetExecutionDetailsHandler : IRequestHandler<GetExecutionDetails, AgentExecutionDetailDto?>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetExecutionDetailsHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<AgentExecutionDetailDto?> Handle(GetExecutionDetails request, CancellationToken cancellationToken)
    {
        var execution = await _executionRepository.GetByIdWithDetailsAsync(
            request.ExecutionId,
            cancellationToken);

        if (execution == null)
        {
            return null;
        }

        return new AgentExecutionDetailDto
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
            Metadata = execution.Metadata,
            AgentName = execution.Agent?.Name,
            ConversationTitle = execution.Conversation?.Title,
            CreatedAt = execution.CreatedAt,
            ModifiedAt = execution.UpdatedAt
        };
    }
}
