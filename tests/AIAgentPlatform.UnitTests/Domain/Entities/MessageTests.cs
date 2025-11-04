using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public class MessageTests
{
    [Fact]
    public void Create_WithValidData_ReturnsMessage()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var role = MessageRole.User;
        var content = "Test message content";

        // Act
        var message = Message.Create(conversationId, role, content);

        // Assert
        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(conversationId, message.ConversationId);
        Assert.Equal(role, message.Role);
        Assert.Equal(content, message.Content);
        Assert.True(message.TokenCount > 0);
    }

    [Fact]
    public void Create_WithEmptyConversationId_ThrowsArgumentException()
    {
        // Arrange
        var role = MessageRole.User;
        var content = "Test content";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Message.Create(Guid.Empty, role, content));

        Assert.Contains("Conversation ID 不能為空", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidContent_ThrowsArgumentException(string? invalidContent)
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var role = MessageRole.User;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Message.Create(conversationId, role, invalidContent!));

        Assert.Contains("訊息內容不能為空", exception.Message);
    }

    [Fact]
    public void Create_WithContentTooLong_ThrowsArgumentException()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var role = MessageRole.User;
        var longContent = new string('a', 32001); // 超過 32000 字元

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Message.Create(conversationId, role, longContent));

        Assert.Contains("訊息內容不能超過 32000 個字元", exception.Message);
    }

    [Fact]
    public void UpdateContent_WithValidContent_UpdatesContent()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), MessageRole.User, "Original content");
        var newContent = "Updated content";

        // Act
        message.UpdateContent(newContent);

        // Assert
        Assert.Equal(newContent, message.Content);
    }

    [Fact]
    public void IsUserMessage_WithUserRole_ReturnsTrue()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), MessageRole.User, "Test");

        // Act & Assert
        Assert.True(message.IsUserMessage());
        Assert.False(message.IsAssistantMessage());
        Assert.False(message.IsSystemMessage());
    }

    [Fact]
    public void IsAssistantMessage_WithAssistantRole_ReturnsTrue()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), MessageRole.Assistant, "Test");

        // Act & Assert
        Assert.False(message.IsUserMessage());
        Assert.True(message.IsAssistantMessage());
        Assert.False(message.IsSystemMessage());
    }

    [Fact]
    public void IsSystemMessage_WithSystemRole_ReturnsTrue()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), MessageRole.System, "Test");

        // Act & Assert
        Assert.False(message.IsUserMessage());
        Assert.False(message.IsAssistantMessage());
        Assert.True(message.IsSystemMessage());
    }

    [Theory]
    [InlineData("Hello", 3)]
    [InlineData("Hello World", 6)]
    [InlineData("A", 1)]
    public void Create_EstimatesTokenCount_Correctly(string content, int expectedTokenCount)
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var role = MessageRole.User;

        // Act
        var message = Message.Create(conversationId, role, content);

        // Assert
        Assert.Equal(expectedTokenCount, message.TokenCount);
    }
}
