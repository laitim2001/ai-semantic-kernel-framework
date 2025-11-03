using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for UpdateAgentCommand
/// </summary>
public sealed class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, AgentDto>
{
    private readonly IAgentRepository _agentRepository;

    public UpdateAgentCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<AgentDto> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        // Get existing agent
        var agent = await _agentRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new AgentNotFoundException(request.Id);

        // Parse model
        var model = LLMModel.From(request.Model);

        // Update agent
        agent.Update(request.Name, request.Description, request.Instructions);
        agent.UpdateModelConfiguration(model, request.Temperature, request.MaxTokens);

        // Persist changes
        await _agentRepository.UpdateAsync(agent, cancellationToken);

        // Map to DTO
        return MapToDto(agent);
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
