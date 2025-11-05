using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class VersionChangeTypeTests
{
    [Theory]
    [InlineData("major")]
    [InlineData("minor")]
    [InlineData("patch")]
    [InlineData("rollback")]
    [InlineData("hotfix")]
    public void From_WithValidType_ShouldReturnVersionChangeType(string type)
    {
        // Act
        var result = VersionChangeType.From(type);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(type);
    }

    [Theory]
    [InlineData("MAJOR")]
    [InlineData("Minor")]
    [InlineData("PATCH")]
    public void From_WithDifferentCasing_ShouldReturnCorrectType(string type)
    {
        // Act
        var result = VersionChangeType.From(type);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(type.ToLowerInvariant());
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithInvalidType_ShouldThrowArgumentException(string type)
    {
        // Act & Assert
        var action = () => VersionChangeType.From(type);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid version change type*");
    }

    [Fact]
    public void Major_ShouldReturnMajorType()
    {
        // Act
        var result = VersionChangeType.Major;

        // Assert
        result.Value.Should().Be("major");
    }

    [Fact]
    public void Minor_ShouldReturnMinorType()
    {
        // Act
        var result = VersionChangeType.Minor;

        // Assert
        result.Value.Should().Be("minor");
    }

    [Fact]
    public void Patch_ShouldReturnPatchType()
    {
        // Act
        var result = VersionChangeType.Patch;

        // Assert
        result.Value.Should().Be("patch");
    }

    [Fact]
    public void Rollback_ShouldReturnRollbackType()
    {
        // Act
        var result = VersionChangeType.Rollback;

        // Assert
        result.Value.Should().Be("rollback");
    }

    [Fact]
    public void Hotfix_ShouldReturnHotfixType()
    {
        // Act
        var result = VersionChangeType.Hotfix;

        // Assert
        result.Value.Should().Be("hotfix");
    }

    [Fact]
    public void Equals_WithSameType_ShouldReturnTrue()
    {
        // Arrange
        var type1 = VersionChangeType.Major;
        var type2 = VersionChangeType.From("major");

        // Act & Assert
        type1.Should().Be(type2);
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var type1 = VersionChangeType.Major;
        var type2 = VersionChangeType.Minor;

        // Act & Assert
        type1.Should().NotBe(type2);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var type = VersionChangeType.Major;

        // Act
        var result = type.ToString();

        // Assert
        result.Should().Be("major");
    }
}
