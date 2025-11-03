namespace AIAgentPlatform.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid operation is attempted on an agent
/// </summary>
public sealed class InvalidAgentOperationException : DomainException
{
    public InvalidAgentOperationException(string message)
        : base(message)
    {
    }

    public InvalidAgentOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
