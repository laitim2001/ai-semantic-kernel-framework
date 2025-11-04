using AIAgentPlatform.Application.Conversations.Commands.CreateConversation;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public class CreateConversationCommandValidatorTests
{
    private readonly CreateConversationCommandValidator _validator;

    public CreateConversationCommandValidatorTests()
    {
        _validator = new CreateConversationCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            AgentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Test Conversation"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyAgentId_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            AgentId = Guid.Empty,
            UserId = Guid.NewGuid(),
            Title = "Test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.AgentId));
    }

    [Fact]
    public void Validate_WithEmptyUserId_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            AgentId = Guid.NewGuid(),
            UserId = Guid.Empty,
            Title = "Test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.UserId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithInvalidTitle_ReturnsInvalid(string invalidTitle)
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            AgentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = invalidTitle
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Title));
    }

    [Fact]
    public void Validate_WithTitleTooLong_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            AgentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = new string('a', 201) // 超過 200 字元
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Title));
    }
}
