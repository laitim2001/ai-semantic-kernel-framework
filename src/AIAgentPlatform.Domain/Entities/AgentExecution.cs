using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents a single execution record of an agent
/// </summary>
public sealed class AgentExecution : BaseEntity
{
    /// <summary>
    /// The agent that performed this execution
    /// </summary>
    public Guid AgentId { get; private set; }

    /// <summary>
    /// The conversation this execution belongs to
    /// </summary>
    public Guid ConversationId { get; private set; }

    /// <summary>
    /// Execution start time
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// Execution end time
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// Execution status
    /// </summary>
    public ExecutionStatus Status { get; private set; }

    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public double? ResponseTimeMs { get; private set; }

    /// <summary>
    /// Total tokens used in this execution
    /// </summary>
    public int? TokensUsed { get; private set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Additional execution metadata (JSON)
    /// </summary>
    public string? Metadata { get; private set; }

    // Navigation properties
    public Agent? Agent { get; private set; }
    public Conversation? Conversation { get; private set; }

    // Private constructor for EF Core
    private AgentExecution()
    {
        Status = ExecutionStatus.Pending;
    }

    /// <summary>
    /// Creates a new agent execution record
    /// </summary>
    public static AgentExecution Create(
        Guid agentId,
        Guid conversationId,
        string? metadata = null)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        if (conversationId == Guid.Empty)
            throw new ArgumentException("Conversation ID cannot be empty", nameof(conversationId));

        return new AgentExecution
        {
            AgentId = agentId,
            ConversationId = conversationId,
            StartTime = DateTime.UtcNow,
            Status = ExecutionStatus.Running,
            Metadata = metadata
        };
    }

    /// <summary>
    /// Marks the execution as completed successfully
    /// </summary>
    public void MarkAsCompleted(int tokensUsed)
    {
        if (tokensUsed <= 0)
            throw new ArgumentException("Tokens used must be greater than 0", nameof(tokensUsed));

        if (Status != ExecutionStatus.Running)
            throw new InvalidOperationException("Execution has already ended");

        EndTime = DateTime.UtcNow;
        Status = ExecutionStatus.Completed;
        TokensUsed = tokensUsed;
        ResponseTimeMs = (EndTime.Value - StartTime).TotalMilliseconds;
        MarkAsModified();
    }

    /// <summary>
    /// Marks the execution as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        if (errorMessage.Length > 2000)
            throw new ArgumentException("Error message cannot exceed 2000 characters", nameof(errorMessage));

        if (Status != ExecutionStatus.Running)
            throw new InvalidOperationException("Execution has already ended");

        EndTime = DateTime.UtcNow;
        Status = ExecutionStatus.Failed;
        ErrorMessage = errorMessage;
        ResponseTimeMs = (EndTime.Value - StartTime).TotalMilliseconds;
        MarkAsModified();
    }

    /// <summary>
    /// Updates execution metadata
    /// </summary>
    public void UpdateMetadata(string metadata)
    {
        Metadata = metadata;
        MarkAsModified();
    }
}
