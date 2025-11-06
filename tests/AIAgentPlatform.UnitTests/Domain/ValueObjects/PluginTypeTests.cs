using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class PluginTypeTests
{
    [Theory]
    [InlineData("tool")]
    [InlineData("function")]
    [InlineData("skill")]
    [InlineData("connector")]
    [InlineData("memory")]
    [InlineData("custom")]
    public void From_WithValidType_ShouldReturnPluginType(string type)
    {
        // Act
        var result = PluginType.From(type);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(type);
    }

    [Theory]
    [InlineData("TOOL")]
    [InlineData("Function")]
    [InlineData("SKILL")]
    public void From_WithDifferentCasing_ShouldReturnCorrectType(string type)
    {
        // Act
        var result = PluginType.From(type);

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
        var action = () => PluginType.From(type);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid plugin type*");
    }

    [Fact]
    public void Tool_ShouldReturnToolType()
    {
        // Act
        var result = PluginType.Tool;

        // Assert
        result.Value.Should().Be("tool");
    }

    [Fact]
    public void Function_ShouldReturnFunctionType()
    {
        // Act
        var result = PluginType.Function;

        // Assert
        result.Value.Should().Be("function");
    }

    [Fact]
    public void Skill_ShouldReturnSkillType()
    {
        // Act
        var result = PluginType.Skill;

        // Assert
        result.Value.Should().Be("skill");
    }

    [Fact]
    public void Connector_ShouldReturnConnectorType()
    {
        // Act
        var result = PluginType.Connector;

        // Assert
        result.Value.Should().Be("connector");
    }

    [Fact]
    public void Memory_ShouldReturnMemoryType()
    {
        // Act
        var result = PluginType.Memory;

        // Assert
        result.Value.Should().Be("memory");
    }

    [Fact]
    public void Custom_ShouldReturnCustomType()
    {
        // Act
        var result = PluginType.Custom;

        // Assert
        result.Value.Should().Be("custom");
    }

    [Fact]
    public void Equals_WithSameType_ShouldReturnTrue()
    {
        // Arrange
        var type1 = PluginType.Tool;
        var type2 = PluginType.From("tool");

        // Act & Assert
        type1.Should().Be(type2);
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var type1 = PluginType.Tool;
        var type2 = PluginType.Function;

        // Act & Assert
        type1.Should().NotBe(type2);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var type = PluginType.Tool;

        // Act
        var result = type.ToString();

        // Assert
        result.Should().Be("tool");
    }
}
