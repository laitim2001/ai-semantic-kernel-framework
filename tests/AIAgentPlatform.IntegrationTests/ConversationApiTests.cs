using System.Net;
using System.Net.Http.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Application.Conversations.Commands.AddMessage;
using AIAgentPlatform.Application.Conversations.Commands.CreateConversation;
using FluentAssertions;

namespace AIAgentPlatform.IntegrationTests;

/// <summary>
/// Integration tests for Conversation CRUD API
/// Tests US 1.2: Conversation and Message management
/// </summary>
public class ConversationApiTests : IClassFixture<WebApplicationFactoryHelper>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactoryHelper _factory;

    public ConversationApiTests(WebApplicationFactoryHelper factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateConversation_WithValidData_ShouldReturnCreatedConversation()
    {
        // Arrange: Create a test agent first
        var agent = await CreateTestAgentAsync();
        var userId = Guid.NewGuid();

        var createCommand = new CreateConversationCommand
        {
            AgentId = agent.Id,
            UserId = userId,
            Title = "Test Conversation"
        };

        // Act: Create conversation
        var response = await _client.PostAsJsonAsync("/api/v1/conversations", createCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var conversation = await response.Content.ReadFromJsonAsync<ConversationDto>();

        conversation.Should().NotBeNull();
        conversation!.Id.Should().NotBe(Guid.Empty);
        conversation.AgentId.Should().Be(agent.Id);
        conversation.UserId.Should().Be(userId);
        conversation.Title.Should().Be("Test Conversation");
        conversation.Status.Should().Be("Active");
        conversation.MessageCount.Should().Be(0);
    }

    [Fact]
    public async Task GetConversationById_WithValidId_ShouldReturnConversation()
    {
        // Arrange: Create a conversation
        var conversation = await CreateTestConversationAsync();

        // Act: Get conversation by ID
        var response = await _client.GetAsync($"/api/v1/conversations/{conversation.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedConversation = await response.Content.ReadFromJsonAsync<ConversationDto>();

        retrievedConversation.Should().NotBeNull();
        retrievedConversation!.Id.Should().Be(conversation.Id);
        retrievedConversation.Title.Should().Be(conversation.Title);
    }

    [Fact]
    public async Task GetConversationById_WithNonexistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonexistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/conversations/{nonexistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetConversations_WithFilters_ShouldReturnFilteredList()
    {
        // Arrange: Create multiple conversations
        var agent = await CreateTestAgentAsync();
        var userId = Guid.NewGuid();

        // Create 3 conversations for the same agent and user
        for (int i = 0; i < 3; i++)
        {
            await CreateTestConversationAsync(agent.Id, userId, $"Conversation {i + 1}");
        }

        // Act: Get conversations filtered by userId and agentId
        var response = await _client.GetAsync($"/api/v1/conversations?userId={userId}&agentId={agent.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conversations = await response.Content.ReadFromJsonAsync<List<ConversationDto>>();

        conversations.Should().NotBeNull();
        conversations!.Should().HaveCountGreaterOrEqualTo(3);
        conversations.Should().OnlyContain(c => c.AgentId == agent.Id && c.UserId == userId);
    }

    [Fact]
    public async Task AddMessage_ToConversation_ShouldIncrementMessageCount()
    {
        // Arrange: Create a conversation
        var conversation = await CreateTestConversationAsync();

        var addMessageCommand = new AddMessageCommand
        {
            ConversationId = conversation.Id,
            Role = "user",
            Content = "Hello, this is a test message"
        };

        // Act: Add message to conversation
        var response = await _client.PostAsJsonAsync(
            $"/api/v1/conversations/{conversation.Id}/messages",
            addMessageCommand
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var message = await response.Content.ReadFromJsonAsync<MessageDto>();

        message.Should().NotBeNull();
        message!.ConversationId.Should().Be(conversation.Id);
        message.Role.Should().Be("user");
        message.Content.Should().Be("Hello, this is a test message");

        // Verify conversation message count increased
        var conversationResponse = await _client.GetAsync($"/api/v1/conversations/{conversation.Id}");
        var updatedConversation = await conversationResponse.Content.ReadFromJsonAsync<ConversationDto>();
        updatedConversation!.MessageCount.Should().Be(1);
    }

    [Fact]
    public async Task AddMessage_WithMismatchedConversationId_ShouldReturnBadRequest()
    {
        // Arrange: Create a conversation
        var conversation = await CreateTestConversationAsync();
        var differentConversationId = Guid.NewGuid();

        var addMessageCommand = new AddMessageCommand
        {
            ConversationId = differentConversationId, // Different from route parameter
            Role = "user",
            Content = "Test message"
        };

        // Act: Try to add message with mismatched ID
        var response = await _client.PostAsJsonAsync(
            $"/api/v1/conversations/{conversation.Id}/messages",
            addMessageCommand
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMessage_ToNonexistentConversation_ShouldReturnNotFound()
    {
        // Arrange
        var nonexistentConversationId = Guid.NewGuid();

        var addMessageCommand = new AddMessageCommand
        {
            ConversationId = nonexistentConversationId,
            Role = "user",
            Content = "Test message"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/v1/conversations/{nonexistentConversationId}/messages",
            addMessageCommand
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetConversations_WithPagination_ShouldRespectPageSize()
    {
        // Arrange: Create multiple conversations
        var agent = await CreateTestAgentAsync();
        var userId = Guid.NewGuid();

        // Create 5 conversations
        for (int i = 0; i < 5; i++)
        {
            await CreateTestConversationAsync(agent.Id, userId, $"Pagination Test {i + 1}");
        }

        // Act: Get conversations with pagination
        var response = await _client.GetAsync(
            $"/api/v1/conversations?userId={userId}&pageNumber=1&pageSize=3"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conversations = await response.Content.ReadFromJsonAsync<List<ConversationDto>>();

        conversations.Should().NotBeNull();
        // Should return at most pageSize items
        conversations!.Count.Should().BeLessOrEqualTo(3);
    }

    #region Helper Methods

    private async Task<AgentDto> CreateTestAgentAsync()
    {
        var createCommand = new CreateAgentCommand
        {
            Name = "Test Agent for Conversations",
            Description = "Testing conversation functionality",
            Model = "gpt-4o",
            Instructions = "You are a test assistant",
            UserId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/agents", createCommand);
        response.EnsureSuccessStatusCode();

        var agent = await response.Content.ReadFromJsonAsync<AgentDto>();
        return agent!;
    }

    private async Task<ConversationDto> CreateTestConversationAsync()
    {
        var agent = await CreateTestAgentAsync();
        return await CreateTestConversationAsync(agent.Id, Guid.NewGuid(), "Test Conversation");
    }

    private async Task<ConversationDto> CreateTestConversationAsync(
        Guid agentId,
        Guid userId,
        string title)
    {
        var createCommand = new CreateConversationCommand
        {
            AgentId = agentId,
            UserId = userId,
            Title = title
        };

        var response = await _client.PostAsJsonAsync("/api/v1/conversations", createCommand);
        response.EnsureSuccessStatusCode();

        var conversation = await response.Content.ReadFromJsonAsync<ConversationDto>();
        return conversation!;
    }

    #endregion
}
