using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class LLMModelTests
{
    [Theory]
    [InlineData("gpt-4")]
    [InlineData("gpt-4o")]
    [InlineData("gpt-4o-mini")]
    [InlineData("GPT-4")] // Test case insensitivity
    [InlineData("GPT-4O-MINI")]
    public void From_WithValidModel_ShouldCreateLLMModel(string modelValue)
    {
        // Act
        var model = LLMModel.From(modelValue);

        // Assert
        model.Should().NotBeNull();
        model.Value.Should().Be(modelValue.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void From_WithEmptyModel_ShouldThrowArgumentException(string modelValue)
    {
        // Act & Assert
        var action = () => LLMModel.From(modelValue);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*LLM model cannot be empty*");
    }

    [Theory]
    [InlineData("invalid-model")]
    [InlineData("gpt-3.5")]
    [InlineData("claude-3")]
    public void From_WithInvalidModel_ShouldThrowArgumentException(string modelValue)
    {
        // Act & Assert
        var action = () => LLMModel.From(modelValue);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid LLM model*");
    }

    [Theory]
    [InlineData("gpt-4", true)]
    [InlineData("gpt-4o", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void IsValid_ShouldReturnCorrectResult(string modelValue, bool expected)
    {
        // Act
        var isValid = LLMModel.IsValid(modelValue);

        // Assert
        isValid.Should().Be(expected);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var model = LLMModel.GPT4o;

        // Act
        string modelString = model;

        // Assert
        modelString.Should().Be("gpt-4o");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var model1 = LLMModel.From("gpt-4o");
        var model2 = LLMModel.From("gpt-4o");

        // Act & Assert
        model1.Equals(model2).Should().BeTrue();
        (model1 == model2).Should().BeFalse(); // Different instances
    }
}
