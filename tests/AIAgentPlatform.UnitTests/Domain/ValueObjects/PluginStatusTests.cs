using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class PluginStatusTests
{
    [Fact]
    public void Active_ShouldReturnActiveStatus()
    {
        // Act
        var status = PluginStatus.Active;

        // Assert
        status.Should().NotBeNull();
        status.Value.Should().Be("active");
        status.IsActive.Should().BeTrue();
        status.IsInactive.Should().BeFalse();
        status.IsFailed.Should().BeFalse();
    }

    [Fact]
    public void Inactive_ShouldReturnInactiveStatus()
    {
        // Act
        var status = PluginStatus.Inactive;

        // Assert
        status.Should().NotBeNull();
        status.Value.Should().Be("inactive");
        status.IsActive.Should().BeFalse();
        status.IsInactive.Should().BeTrue();
        status.IsFailed.Should().BeFalse();
    }

    [Fact]
    public void Failed_ShouldReturnFailedStatus()
    {
        // Act
        var status = PluginStatus.Failed;

        // Assert
        status.Should().NotBeNull();
        status.Value.Should().Be("failed");
        status.IsActive.Should().BeFalse();
        status.IsInactive.Should().BeFalse();
        status.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData("active")]
    [InlineData("ACTIVE")]
    [InlineData("Active")]
    public void From_WithValidActiveStatus_ShouldReturnActiveStatus(string value)
    {
        // Act
        var status = PluginStatus.From(value);

        // Assert
        status.Should().Be(PluginStatus.Active);
        status.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("inactive")]
    [InlineData("INACTIVE")]
    [InlineData("Inactive")]
    public void From_WithValidInactiveStatus_ShouldReturnInactiveStatus(string value)
    {
        // Act
        var status = PluginStatus.From(value);

        // Assert
        status.Should().Be(PluginStatus.Inactive);
        status.IsInactive.Should().BeTrue();
    }

    [Theory]
    [InlineData("failed")]
    [InlineData("FAILED")]
    [InlineData("Failed")]
    public void From_WithValidFailedStatus_ShouldReturnFailedStatus(string value)
    {
        // Act
        var status = PluginStatus.From(value);

        // Assert
        status.Should().Be(PluginStatus.Failed);
        status.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void From_WithEmptyValue_ShouldThrowArgumentException(string? value)
    {
        // Act & Assert
        var action = () => PluginStatus.From(value!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*status cannot be empty*");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("pending")]
    [InlineData("archived")]
    public void From_WithInvalidValue_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        var action = () => PluginStatus.From(value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid plugin status*");
    }

    [Theory]
    [InlineData("active", true)]
    [InlineData("inactive", true)]
    [InlineData("failed", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void IsValid_ShouldReturnExpectedResult(string value, bool expected)
    {
        // Act
        var result = PluginStatus.IsValid(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var status = PluginStatus.Active;

        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be("active");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var status = PluginStatus.Active;

        // Act
        string result = status;

        // Assert
        result.Should().Be("active");
    }

    [Fact]
    public void Equals_WithSameStatus_ShouldReturnTrue()
    {
        // Arrange
        var status1 = PluginStatus.Active;
        var status2 = PluginStatus.Active;

        // Act & Assert
        status1.Equals(status2).Should().BeTrue();
        (status1 == status2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentStatus_ShouldReturnFalse()
    {
        // Arrange
        var status1 = PluginStatus.Active;
        var status2 = PluginStatus.Inactive;

        // Act & Assert
        status1.Equals(status2).Should().BeFalse();
        (status1 == status2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var status1 = PluginStatus.Active;
        var status2 = PluginStatus.Active;

        // Act & Assert
        status1.GetHashCode().Should().Be(status2.GetHashCode());
    }
}
