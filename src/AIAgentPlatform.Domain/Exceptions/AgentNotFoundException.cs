namespace AIAgentPlatform.Domain.Exceptions;

/// <summary>
/// Exception thrown when an agent cannot be found
/// </summary>
public sealed class AgentNotFoundException : DomainException
{
    public Guid AgentId { get; }

    public AgentNotFoundException(Guid agentId)
        : base($"Agent with ID '{agentId}' was not found.")
    {
        AgentId = agentId;
    }
}
