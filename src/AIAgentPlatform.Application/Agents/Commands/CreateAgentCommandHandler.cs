using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for CreateAgentCommand
/// </summary>
public sealed class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, AgentDto>
{
    private readonly IAgentRepository _agentRepository;

    public CreateAgentCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<AgentDto> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        // Parse model
        var model = LLMModel.From(request.Model);

        // Create agent entity
        var agent = Agent.Create(
            userId: request.UserId,
            name: request.Name,
            instructions: request.Instructions,
            model: model,
            temperature: request.Temperature,
            maxTokens: request.MaxTokens,
            description: request.Description
        );

        // Persist to database
        var createdAgent = await _agentRepository.AddAsync(agent, cancellationToken);

        // Map to DTO
        return MapToDto(createdAgent);
    }

    private static AgentDto MapToDto(Agent agent)
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
