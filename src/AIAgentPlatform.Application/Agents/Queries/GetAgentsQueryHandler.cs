using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// Handler for GetAgentsQuery
/// </summary>
public sealed class GetAgentsQueryHandler : IRequestHandler<GetAgentsQuery, GetAgentsQueryResult>
{
    private readonly IAgentRepository _agentRepository;

    public GetAgentsQueryHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<GetAgentsQueryResult> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
    {
        // Validate and limit Take parameter
        var take = Math.Min(request.Take, 100); // Max 100 records per page

        // Get agents with filtering, searching, sorting, and pagination
        var agents = await _agentRepository.GetAllAsync(
            userId: request.UserId,
            status: request.Status,
            searchTerm: request.SearchTerm,
            model: request.Model,
            sortBy: request.SortBy,
            sortOrder: request.SortOrder,
            skip: request.Skip,
            take: take,
            cancellationToken: cancellationToken
        );

        // Get total count with same filters and search
        var totalCount = await _agentRepository.GetCountAsync(
            userId: request.UserId,
            status: request.Status,
            searchTerm: request.SearchTerm,
            model: request.Model,
            cancellationToken: cancellationToken
        );

        // Map to DTOs
        var agentDtos = agents.Select(MapToDto).ToList();

        return new GetAgentsQueryResult
        {
            Agents = agentDtos,
            TotalCount = totalCount,
            Skip = request.Skip,
            Take = take
        };
    }

    private static AgentDto MapToDto(AIAgentPlatform.Domain.Entities.Agent agent)
    {
        return new AgentDto
        {
            Id = agent.Id,
            UserId = agent.UserId,
            Name = agent.Name,
            Description = agent.Description,
            Instructions = agent.Instructions,
            Model = agent.Model.Value,
            Temperature = agent.Temperature,
            MaxTokens = agent.MaxTokens,
            Status = agent.Status.Value,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt
        };
    }
}
