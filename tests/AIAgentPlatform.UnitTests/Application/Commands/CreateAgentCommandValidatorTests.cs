using AIAgentPlatform.Application.Agents.Commands;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class CreateAgentCommandValidatorTests
{
    private readonly CreateAgentCommandValidator _validator;

    public CreateAgentCommandValidatorTests()
    {
        _validator = new CreateAgentCommandValidator();
    }

    [Fact]
    public async Task Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyName_ShouldFail(string name)
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = name,
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task Validate_WithNameTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = new string('a', 101),
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyInstructions_ShouldFail(string instructions)
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = instructions,
            Model = "gpt-4o"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Instructions));
    }

    [Theory]
    [InlineData("invalid-model")]
    [InlineData("gpt-3.5")]
    public async Task Validate_WithInvalidModel_ShouldFail(string model)
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = "You are a helpful assistant",
            Model = model
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Model));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(2.1)]
    public async Task Validate_WithInvalidTemperature_ShouldFail(decimal temperature)
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o",
            Temperature = temperature
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Temperature));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(128001)]
    public async Task Validate_WithInvalidMaxTokens_ShouldFail(int maxTokens)
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o",
            MaxTokens = maxTokens
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.MaxTokens));
    }
}
