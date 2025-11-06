using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.AgentExecutions.Queries;

/// <summary>
/// Handler for GetExecutionHistory query
/// </summary>
public sealed class GetExecutionHistoryHandler : IRequestHandler<GetExecutionHistory, PagedResultDto<AgentExecutionDto>>
{
    private readonly IAgentExecutionRepository _executionRepository;

    public GetExecutionHistoryHandler(IAgentExecutionRepository executionRepository)
    {
        _executionRepository = executionRepository;
    }

    public async Task<PagedResultDto<AgentExecutionDto>> Handle(GetExecutionHistory request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _executionRepository.GetByAgentIdAsync(
            request.AgentId,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.ConversationId,
            request.MinTokens,
            request.MaxTokens,
            request.MinResponseTimeMs,
            request.MaxResponseTimeMs,
            request.SearchTerm,
            request.SortBy,
            request.SortDescending,
            request.Skip,
            request.Take,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        var pageSize = request.Take;
        var currentPage = request.Skip / pageSize + 1;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResultDto<AgentExecutionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            HasPreviousPage = currentPage > 1,
            HasNextPage = currentPage < totalPages
        };
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
