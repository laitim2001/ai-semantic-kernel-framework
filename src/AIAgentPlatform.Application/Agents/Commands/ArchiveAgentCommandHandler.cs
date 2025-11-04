using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for ArchiveAgentCommand
/// </summary>
public sealed class ArchiveAgentCommandHandler : IRequestHandler<ArchiveAgentCommand, AgentDto>
{
    private readonly IAgentRepository _agentRepository;

    public ArchiveAgentCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<AgentDto> Handle(ArchiveAgentCommand request, CancellationToken cancellationToken)
    {
        // Get existing agent
        var agent = await _agentRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new AgentNotFoundException(request.Id);

        // Archive agent
        agent.Archive();

        // Persist changes
        await _agentRepository.UpdateAsync(agent, cancellationToken);

        // Map to DTO
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
