using System.Net;
using System.Net.Http.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Application.Agents.DTOs;
using FluentAssertions;

namespace AIAgentPlatform.IntegrationTests;

/// <summary>
/// Integration tests for Agent Execution Statistics API
/// Tests US 1.3 Phase 2: Agent execution tracking and statistics
/// </summary>
public class AgentExecutionApiTests : IClassFixture<WebApplicationFactoryHelper>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactoryHelper _factory;

    public AgentExecutionApiTests(WebApplicationFactoryHelper factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStatistics_WithValidAgentId_ShouldReturnStatistics()
    {
        // Arrange: Create a test agent
        var createCommand = new CreateAgentCommand
        {
            Name = "Test Agent for Statistics",
            Description = "Testing execution statistics",
            Model = "gpt-4o",
            Instructions = "You are a test assistant",
            UserId = Guid.NewGuid()
        };

        var createResponse = await _client.PostAsJsonAsync("/api/agents", createCommand);
        createResponse.EnsureSuccessStatusCode();
        var agent = await createResponse.Content.ReadFromJsonAsync<AgentDto>();
        agent.Should().NotBeNull();

        // Act: Get statistics for the agent
        var statisticsResponse = await _client.GetAsync($"/api/agents/{agent!.Id}/statistics");

        // Assert
        statisticsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await statisticsResponse.Content.ReadFromJsonAsync<AgentStatisticsDto>();

        statistics.Should().NotBeNull();
        statistics!.AgentId.Should().Be(agent.Id);
        statistics.TotalExecutions.Should().Be(0); // New agent, no executions yet
        statistics.SuccessfulExecutions.Should().Be(0);
        statistics.FailedExecutions.Should().Be(0);
    }

    [Fact]
    public async Task GetStatistics_WithDateRange_ShouldFilterByDateRange()
    {
        // Arrange: Create a test agent
        var createCommand = new CreateAgentCommand
        {
            Name = "Test Agent for Date Range",
            Description = "Testing statistics with date range",
            Model = "gpt-4o",
            Instructions = "You are a test assistant",
            UserId = Guid.NewGuid()
        };

        var createResponse = await _client.PostAsJsonAsync("/api/agents", createCommand);
        createResponse.EnsureSuccessStatusCode();
        var agent = await createResponse.Content.ReadFromJsonAsync<AgentDto>();

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act: Get statistics with date range
        var statisticsResponse = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/statistics?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        statisticsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await statisticsResponse.Content.ReadFromJsonAsync<AgentStatisticsDto>();

        statistics.Should().NotBeNull();
        statistics!.AgentId.Should().Be(agent.Id);
        statistics.PeriodStart.Should().BeCloseTo(startDate, TimeSpan.FromSeconds(1));
        statistics.PeriodEnd.Should().BeCloseTo(endDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetStatistics_WithNonexistentAgent_ShouldReturnNotFound()
    {
        // Arrange
        var nonexistentAgentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/agents/{nonexistentAgentId}/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStatistics_WithInvalidDateRange_ShouldReturnBadRequest()
    {
        // Arrange: Create a test agent
        var createCommand = new CreateAgentCommand
        {
            Name = "Test Agent for Invalid Date Range",
            Description = "Testing invalid date range",
            Model = "gpt-4o",
            Instructions = "You are a test assistant",
            UserId = Guid.NewGuid()
        };

        var createResponse = await _client.PostAsJsonAsync("/api/agents", createCommand);
        createResponse.EnsureSuccessStatusCode();
        var agent = await createResponse.Content.ReadFromJsonAsync<AgentDto>();

        // Act: End date before start date
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-7);
        var statisticsResponse = await _client.GetAsync(
            $"/api/agents/{agent!.Id}/statistics?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        statisticsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
