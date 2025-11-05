using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to create a new agent version
/// </summary>
public sealed record CreateAgentVersionCommand : IRequest<Guid>
{
    public Guid AgentId { get; init; }
    public Guid UserId { get; init; }
    public string ChangeDescription { get; init; } = string.Empty;
    public string ChangeType { get; init; } = "minor";
}
