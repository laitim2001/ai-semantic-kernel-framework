using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to rollback agent to a specific version
/// </summary>
public sealed record RollbackAgentVersionCommand : IRequest<bool>
{
    public Guid AgentId { get; init; }
    public Guid VersionId { get; init; }
    public Guid UserId { get; init; }
}
