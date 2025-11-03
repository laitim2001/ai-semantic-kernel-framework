using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Queries;

/// <summary>
/// Handler for GetAgentByIdQuery
/// </summary>
public sealed class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDto?>
{
    private readonly IAgentRepository _agentRepository;

    public GetAgentByIdQueryHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<AgentDto?> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var agent = await _agentRepository.GetByIdAsync(request.Id, cancellationToken);

        return agent == null ? null : MapToDto(agent);
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
