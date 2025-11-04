namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Result of a batch operation on agents
/// </summary>
public sealed record BatchOperationResult
{
    /// <summary>
    /// Total number of agents attempted
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Number of successfully processed agents
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Number of failed agents
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// IDs of successfully processed agents
    /// </summary>
    public List<Guid> SuccessfulIds { get; init; } = new();

    /// <summary>
    /// Failed agent IDs with error messages
    /// </summary>
    public List<BatchOperationError> Errors { get; init; } = new();
}

/// <summary>
/// Error information for a failed agent operation
/// </summary>
public sealed record BatchOperationError
{
    /// <summary>
    /// Agent ID that failed
    /// </summary>
    public Guid AgentId { get; init; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;
}
