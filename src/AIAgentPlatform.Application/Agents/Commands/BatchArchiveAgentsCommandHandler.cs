using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Handler for BatchArchiveAgentsCommand
/// </summary>
public sealed class BatchArchiveAgentsCommandHandler : IRequestHandler<BatchArchiveAgentsCommand, BatchOperationResult>
{
    private readonly IAgentRepository _agentRepository;

    public BatchArchiveAgentsCommandHandler(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<BatchOperationResult> Handle(BatchArchiveAgentsCommand request, CancellationToken cancellationToken)
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

                agent.Archive();
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
