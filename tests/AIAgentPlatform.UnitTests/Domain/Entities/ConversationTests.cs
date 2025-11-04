using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public class ConversationTests
{
    [Fact]
    public void Create_WithValidData_ReturnsConversation()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Conversation";

        // Act
        var conversation = Conversation.Create(agentId, userId, title);

        // Assert
        Assert.NotEqual(Guid.Empty, conversation.Id);
        Assert.Equal(agentId, conversation.AgentId);
        Assert.Equal(userId, conversation.UserId);
        Assert.Equal(title, conversation.Title);
        Assert.Equal(ConversationStatus.Active, conversation.Status);
        Assert.Equal(0, conversation.MessageCount);
        Assert.Null(conversation.LastMessageAt);
    }

    [Fact]
    public void Create_WithEmptyAgentId_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Test Conversation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Conversation.Create(Guid.Empty, userId, title));

        Assert.Contains("Agent ID 不能為空", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var title = "Test Conversation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Conversation.Create(agentId, Guid.Empty, title));

        Assert.Contains("User ID 不能為空", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Conversation.Create(agentId, userId, invalidTitle!));

        Assert.Contains("對話標題不能為空", exception.Message);
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_UpdatesTitle()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Original Title");
        var newTitle = "Updated Title";

        // Act
        conversation.UpdateTitle(newTitle);

        // Assert
        Assert.Equal(newTitle, conversation.Title);
    }

    [Fact]
    public void Archive_ActiveConversation_ChangesStatusToArchived()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");

        // Act
        conversation.Archive();

        // Assert
        Assert.Equal(ConversationStatus.Archived, conversation.Status);
    }

    [Fact]
    public void Activate_ArchivedConversation_ChangesStatusToActive()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");
        conversation.Archive();

        // Act
        conversation.Activate();

        // Assert
        Assert.Equal(ConversationStatus.Active, conversation.Status);
    }

    [Fact]
    public void Delete_ActiveConversation_ChangesStatusToDeleted()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");

        // Act
        conversation.Delete();

        // Assert
        Assert.Equal(ConversationStatus.Deleted, conversation.Status);
    }

    [Fact]
    public void AddMessage_ToActiveConversation_AddsMessageAndUpdatesCount()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");
        var message = Message.Create(conversation.Id, MessageRole.User, "Hello");

        // Act
        conversation.AddMessage(message);

        // Assert
        Assert.Equal(1, conversation.MessageCount);
        Assert.NotNull(conversation.LastMessageAt);
        Assert.Contains(message, conversation.Messages);
    }

    [Fact]
    public void AddMessage_ToNonActiveConversation_ThrowsInvalidOperationException()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");
        conversation.Archive();
        var message = Message.Create(conversation.Id, MessageRole.User, "Hello");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => conversation.AddMessage(message));
    }

    [Fact]
    public void CanAddMessage_ActiveConversation_ReturnsTrue()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");

        // Act & Assert
        Assert.True(conversation.CanAddMessage());
    }

    [Fact]
    public void CanAddMessage_ArchivedConversation_ReturnsFalse()
    {
        // Arrange
        var conversation = Conversation.Create(Guid.NewGuid(), Guid.NewGuid(), "Test");
        conversation.Archive();

        // Act & Assert
        Assert.False(conversation.CanAddMessage());
    }
}
