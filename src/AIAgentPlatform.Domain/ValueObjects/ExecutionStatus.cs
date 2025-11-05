namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents the status of an agent execution
/// </summary>
public sealed class ExecutionStatus
{
    public string Value { get; }

    // Predefined statuses
    public static readonly ExecutionStatus Pending = new("pending");
    public static readonly ExecutionStatus Running = new("running");
    public static readonly ExecutionStatus Completed = new("completed");
    public static readonly ExecutionStatus Failed = new("failed");
    public static readonly ExecutionStatus Cancelled = new("cancelled");

    private static readonly HashSet<string> ValidStatuses = new()
    {
        "pending",
        "running",
        "completed",
        "failed",
        "cancelled"
    };

    private ExecutionStatus(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new ExecutionStatus from string value
    /// </summary>
    public static ExecutionStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Execution status cannot be empty", nameof(value));

        var normalizedValue = value.ToLowerInvariant();

        if (!ValidStatuses.Contains(normalizedValue))
            throw new ArgumentException($"Invalid execution status: {value}. Valid statuses: {string.Join(", ", ValidStatuses)}", nameof(value));

        return new ExecutionStatus(normalizedValue);
    }

    /// <summary>
    /// Checks if a status string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidStatuses.Contains(value.ToLowerInvariant());
    }

    public bool IsPending => Value == "pending";
    public bool IsRunning => Value == "running";
    public bool IsCompleted => Value == "completed";
    public bool IsFailed => Value == "failed";
    public bool IsCancelled => Value == "cancelled";

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is ExecutionStatus other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(ExecutionStatus status) => status.Value;
}
