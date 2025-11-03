using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for DeleteAgentCommand
/// </summary>
public sealed class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand, Unit>
{
    private readonly IAgentRepository _agentRepository;

    public DeleteAgentCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<Unit> Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
    {
        // Get existing agent
        var agent = await _agentRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new AgentNotFoundException(request.Id);

        // Archive (soft delete)
        agent.Archive();

        // Persist changes
        await _agentRepository.UpdateAsync(agent, cancellationToken);

        return Unit.Value;
    }
}
