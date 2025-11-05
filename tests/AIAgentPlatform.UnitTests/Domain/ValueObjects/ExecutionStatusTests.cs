using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class ExecutionStatusTests
{
    [Theory]
    [InlineData("pending")]
    [InlineData("running")]
    [InlineData("completed")]
    [InlineData("failed")]
    [InlineData("cancelled")]
    public void From_WithValidStatus_ShouldReturnExecutionStatus(string status)
    {
        // Act
        var result = ExecutionStatus.From(status);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(status);
    }

    [Theory]
    [InlineData("PENDING")]
    [InlineData("Running")]
    [InlineData("COMPLETED")]
    public void From_WithDifferentCasing_ShouldReturnCorrectStatus(string status)
    {
        // Act
        var result = ExecutionStatus.From(status);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(status.ToLowerInvariant());
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithInvalidStatus_ShouldThrowArgumentException(string status)
    {
        // Act & Assert
        var action = () => ExecutionStatus.From(status);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid execution status*");
    }

    [Fact]
    public void Pending_ShouldReturnPendingStatus()
    {
        // Act
        var result = ExecutionStatus.Pending;

        // Assert
        result.Value.Should().Be("pending");
    }

    [Fact]
    public void Running_ShouldReturnRunningStatus()
    {
        // Act
        var result = ExecutionStatus.Running;

        // Assert
        result.Value.Should().Be("running");
    }

    [Fact]
    public void Completed_ShouldReturnCompletedStatus()
    {
        // Act
        var result = ExecutionStatus.Completed;

        // Assert
        result.Value.Should().Be("completed");
    }

    [Fact]
    public void Failed_ShouldReturnFailedStatus()
    {
        // Act
        var result = ExecutionStatus.Failed;

        // Assert
        result.Value.Should().Be("failed");
    }

    [Fact]
    public void Cancelled_ShouldReturnCancelledStatus()
    {
        // Act
        var result = ExecutionStatus.Cancelled;

        // Assert
        result.Value.Should().Be("cancelled");
    }

    [Fact]
    public void Equals_WithSameStatus_ShouldReturnTrue()
    {
        // Arrange
        var status1 = ExecutionStatus.Running;
        var status2 = ExecutionStatus.From("running");

        // Act & Assert
        status1.Should().Be(status2);
    }

    [Fact]
    public void Equals_WithDifferentStatus_ShouldReturnFalse()
    {
        // Arrange
        var status1 = ExecutionStatus.Running;
        var status2 = ExecutionStatus.Completed;

        // Act & Assert
        status1.Should().NotBe(status2);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var status = ExecutionStatus.Running;

        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be("running");
    }
}
