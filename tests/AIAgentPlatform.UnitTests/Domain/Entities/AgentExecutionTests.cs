using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public sealed class AgentExecutionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAgentExecution()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var metadata = "{\"source\":\"api\"}";

        // Act
        var execution = AgentExecution.Create(agentId, conversationId, metadata);

        // Assert
        execution.Should().NotBeNull();
        execution.AgentId.Should().Be(agentId);
        execution.ConversationId.Should().Be(conversationId);
        execution.Metadata.Should().Be(metadata);
        execution.Status.Should().Be(ExecutionStatus.Running);
        execution.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        execution.EndTime.Should().BeNull();
        execution.ResponseTimeMs.Should().BeNull();
        execution.TokensUsed.Should().BeNull();
        execution.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullMetadata_ShouldCreateAgentExecution()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        // Act
        var execution = AgentExecution.Create(agentId, conversationId);

        // Assert
        execution.Should().NotBeNull();
        execution.Metadata.Should().BeNull();
    }

    [Fact]
    public void MarkAsCompleted_WithValidTokens_ShouldUpdateExecutionStatus()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        var tokensUsed = 1500;

        // Act
        execution.MarkAsCompleted(tokensUsed);

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Completed);
        execution.TokensUsed.Should().Be(tokensUsed);
        execution.EndTime.Should().NotBeNull();
        execution.EndTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        execution.ResponseTimeMs.Should().BeGreaterThan(0);
        execution.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void MarkAsCompleted_WithZeroTokens_ShouldThrowArgumentException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var action = () => execution.MarkAsCompleted(0);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Tokens used must be greater than 0*");
    }

    [Fact]
    public void MarkAsCompleted_WithNegativeTokens_ShouldThrowArgumentException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var action = () => execution.MarkAsCompleted(-100);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Tokens used must be greater than 0*");
    }

    [Fact]
    public void MarkAsFailed_WithValidErrorMessage_ShouldUpdateExecutionStatus()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        var errorMessage = "Connection timeout";

        // Act
        execution.MarkAsFailed(errorMessage);

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Failed);
        execution.ErrorMessage.Should().Be(errorMessage);
        execution.EndTime.Should().NotBeNull();
        execution.EndTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        execution.ResponseTimeMs.Should().BeGreaterThan(0);
        execution.TokensUsed.Should().BeNull();
    }

    [Fact]
    public void MarkAsFailed_WithEmptyErrorMessage_ShouldThrowArgumentException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var action = () => execution.MarkAsFailed("");
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Error message cannot be empty*");
    }

    [Fact]
    public void MarkAsFailed_WithErrorMessageTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        var errorMessage = new string('x', 2001);

        // Act & Assert
        var action = () => execution.MarkAsFailed(errorMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Error message cannot exceed 2000 characters*");
    }

    [Fact]
    public void MarkAsCompleted_AfterAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        execution.MarkAsCompleted(1000);

        // Act & Assert
        var action = () => execution.MarkAsCompleted(1500);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Execution has already ended*");
    }

    [Fact]
    public void MarkAsFailed_AfterAlreadyFailed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        execution.MarkAsFailed("First error");

        // Act & Assert
        var action = () => execution.MarkAsFailed("Second error");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Execution has already ended*");
    }

    [Fact]
    public void MarkAsFailed_AfterCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var execution = AgentExecution.Create(Guid.NewGuid(), Guid.NewGuid());
        execution.MarkAsCompleted(1000);

        // Act & Assert
        var action = () => execution.MarkAsFailed("Error");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Execution has already ended*");
    }
}
