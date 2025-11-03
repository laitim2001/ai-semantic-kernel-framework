using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Command to delete an agent (archive/soft delete)
/// </summary>
public sealed record DeleteAgentCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
