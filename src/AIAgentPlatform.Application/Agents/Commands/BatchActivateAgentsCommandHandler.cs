using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for BatchActivateAgentsCommand
/// </summary>
public sealed class BatchActivateAgentsCommandHandler : IRequestHandler<BatchActivateAgentsCommand, BatchOperationResult>
{
    private readonly IAgentRepository _agentRepository;

    public BatchActivateAgentsCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<BatchOperationResult> Handle(BatchActivateAgentsCommand request, CancellationToken cancellationToken)
    {
        var successfulIds = new List<Guid>();
        var errors = new List<BatchOperationError>();

        foreach (var agentId in request.AgentIds)
        {
            try
            {
                var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);

                if (agent == null)
                {
                    errors.Add(new BatchOperationError
                    {
                        AgentId = agentId,
                        ErrorMessage = "Agent not found"
                    });
                    continue;
                }

                agent.Activate();
                await _agentRepository.UpdateAsync(agent, cancellationToken);
                successfulIds.Add(agentId);
            }
            catch (Exception ex)
            {
                errors.Add(new BatchOperationError
                {
                    AgentId = agentId,
                    ErrorMessage = ex.Message
                });
            }
        }

        return new BatchOperationResult
        {
            TotalCount = request.AgentIds.Count,
            SuccessCount = successfulIds.Count,
            FailureCount = errors.Count,
            SuccessfulIds = successfulIds,
            Errors = errors
        };
    }
}
