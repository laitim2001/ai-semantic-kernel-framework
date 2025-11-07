using System.Net;
using System.Net.Http.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Application.Conversations.Commands.CreateConversation;
using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.API.Controllers;
using FluentAssertions;

namespace AIAgentPlatform.IntegrationTests;

/// <summary>
/// Integration tests for Agent Execution API
/// Tests US 1.4 Phase 1: Basic execution engine and API endpoints
/// </summary>
public sealed class AgentExecutionApiTests : IClassFixture<WebApplicationFactoryHelper>
{
    private readonly WebApplicationFactoryHelper _factory;
    private readonly HttpClient _client;

    public AgentExecutionApiTests(WebApplicationFactoryHelper factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetExecutionHistory_WithValidAgentId_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Create an agent first
        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Execution History",
            Description = "Testing execution history",
            Instructions = "You are a test assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/agents/{agent!.Id}/AgentExecution/history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<AgentExecutionDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty(); // No executions yet
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetExecutionHistory_WithPagination_ShouldRespectPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Pagination",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        // Act
        var response = await _client.GetAsync($"/api/agents/{agent!.Id}/AgentExecution/history?skip=0&take=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<AgentExecutionDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountLessOrEqualTo(10);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetStatistics_WithValidAgentId_ShouldReturnZeroStatistics()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Statistics",
            Description = "Testing statistics",
            Instructions = "You are a test assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/agents/{agent!.Id}/AgentExecution/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await response.Content.ReadFromJsonAsync<AgentExecutionStatisticsDto>();
        statistics.Should().NotBeNull();
        statistics!.AgentId.Should().Be(agent.Id);
        statistics.TotalExecutions.Should().Be(0);
        statistics.SuccessfulExecutions.Should().Be(0);
        statistics.FailedExecutions.Should().Be(0);
        statistics.SuccessRate.Should().Be(0);
    }

    [Fact]
    public async Task GetStatistics_WithDateRange_ShouldIncludeDateRange()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Date Range",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/statistics?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await response.Content.ReadFromJsonAsync<AgentExecutionStatisticsDto>();
        statistics.Should().NotBeNull();
        statistics!.StartDate.Should().NotBeNull();
        statistics.EndDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Execute_WithNonExistentAgent_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentAgentId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var executeRequest = new ExecuteAgentRequest
        {
            ConversationId = conversationId,
            Input = "Hello, world!"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/agents/{nonExistentAgentId}/AgentExecution/execute",
            executeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Execute_WithPausedAgent_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Create an agent
        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Execution",
            Description = "Testing execution",
            Instructions = "You are a test assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        // Pause the agent
        await _client.PostAsync($"/api/agents/{agent!.Id}/pause", null);

        // Create a conversation
        var createConversationCommand = new CreateConversationCommand
        {
            UserId = userId,
            AgentId = agent.Id,
            Title = "Test Conversation"
        };

        var createConversationResponse = await _client.PostAsJsonAsync("/api/conversations", createConversationCommand);
        createConversationResponse.EnsureSuccessStatusCode();
        var conversation = await createConversationResponse.Content.ReadFromJsonAsync<ConversationDto>();

        var executeRequest = new ExecuteAgentRequest
        {
            ConversationId = conversation!.Id,
            Input = "Hello, world!"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/agents/{agent.Id}/AgentExecution/execute",
            executeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Execute_WithEmptyInput_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Create an agent
        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        var executeRequest = new ExecuteAgentRequest
        {
            ConversationId = Guid.NewGuid(),
            Input = "" // Empty input
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/execute",
            executeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStatistics_WithEnhancedMetrics_ShouldIncludeDetailedMetrics()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Enhanced Statistics",
            Description = "Testing enhanced performance metrics",
            Instructions = "You are a test assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/agents/{agent!.Id}/AgentExecution/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await response.Content.ReadFromJsonAsync<AgentExecutionStatisticsDto>();
        statistics.Should().NotBeNull();
        statistics!.AgentId.Should().Be(agent.Id);

        // Verify all enhanced metrics are included
        statistics.TotalExecutions.Should().Be(0);
        statistics.SuccessfulExecutions.Should().Be(0);
        statistics.FailedExecutions.Should().Be(0);
        statistics.CancelledExecutions.Should().Be(0);
        statistics.SuccessRate.Should().Be(0);
        statistics.AverageResponseTimeMs.Should().Be(0);

        // Enhanced metrics should be null for no data
        statistics.MinResponseTimeMs.Should().BeNull();
        statistics.MaxResponseTimeMs.Should().BeNull();
        statistics.MedianResponseTimeMs.Should().BeNull();
        statistics.P95ResponseTimeMs.Should().BeNull();
        statistics.P99ResponseTimeMs.Should().BeNull();
        statistics.TotalTokensUsed.Should().Be(0);
        statistics.AverageTokensPerExecution.Should().Be(0);
        statistics.MinTokensUsed.Should().BeNull();
        statistics.MaxTokensUsed.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesStatistics_WithValidGranularity_ShouldReturnTimeSeriesData()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Time Series",
            Description = "Testing time series statistics",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        createAgentResponse.EnsureSuccessStatusCode();
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/statistics/timeseries?startDate={startDate:O}&endDate={endDate:O}&granularity=day");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var timeSeries = await response.Content.ReadFromJsonAsync<TimeSeriesStatisticsDto>();
        timeSeries.Should().NotBeNull();
        timeSeries!.AgentId.Should().Be(agent.Id);
        timeSeries.Granularity.Should().Be("day");
        timeSeries.StartDate.Should().Be(startDate);
        timeSeries.EndDate.Should().Be(endDate);
        timeSeries.DataPoints.Should().NotBeNull();
        timeSeries.TotalExecutions.Should().Be(0);
        timeSeries.OverallSuccessRate.Should().Be(0);
        timeSeries.OverallAverageResponseTimeMs.Should().Be(0);
        timeSeries.OverallTotalTokensUsed.Should().Be(0);
    }

    [Theory]
    [InlineData("hour")]
    [InlineData("day")]
    [InlineData("week")]
    [InlineData("month")]
    public async Task GetTimeSeriesStatistics_WithAllValidGranularities_ShouldSucceed(string granularity)
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = $"Test Agent for {granularity}",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/statistics/timeseries?startDate={startDate:O}&endDate={endDate:O}&granularity={granularity}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var timeSeries = await response.Content.ReadFromJsonAsync<TimeSeriesStatisticsDto>();
        timeSeries.Should().NotBeNull();
        timeSeries!.Granularity.Should().Be(granularity);
    }

    [Fact]
    public async Task GetTimeSeriesStatistics_WithInvalidGranularity_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Invalid Granularity",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/statistics/timeseries?startDate={startDate:O}&endDate={endDate:O}&granularity=invalid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTimeSeriesStatistics_WithEndDateBeforeStartDate_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var createAgentCommand = new CreateAgentCommand
        {
            UserId = userId,
            Name = "Test Agent for Invalid Date Range",
            Instructions = "You are a test assistant",
            Model = "gpt-4o"
        };

        var createAgentResponse = await _client.PostAsJsonAsync("/api/agents", createAgentCommand);
        var agent = await createAgentResponse.Content.ReadFromJsonAsync<AgentDto>();

        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-7);

        // Act
        var response = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/AgentExecution/statistics/timeseries?startDate={startDate:O}&endDate={endDate:O}&granularity=day");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Note: We cannot test actual execution without a valid Azure OpenAI configuration
    // That would require integration with real Azure services, which is beyond unit/integration testing
    // For real execution testing, we would need:
    // 1. Valid Azure OpenAI API credentials
    // 2. Mocked Semantic Kernel service OR
    // 3. End-to-end testing environment with real Azure resources
}
