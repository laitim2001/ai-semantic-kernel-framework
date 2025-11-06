using System.Net;
using System.Net.Http.Json;
using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AIAgentPlatform.IntegrationTests;

/// <summary>
/// Integration tests for Agent Plugin Management API
/// Tests US 1.3 Phase 4: Agent plugin system
/// </summary>
public class AgentPluginApiTests : IClassFixture<WebApplicationFactoryHelper>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactoryHelper _factory;

    public AgentPluginApiTests(WebApplicationFactoryHelper factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddPluginToAgent_WithValidData_ShouldSucceed()
    {
        // Arrange: Create agent and plugin
        var agent = await CreateTestAgentAsync();
        var plugin = await CreateTestPluginAsync();

        var addPluginCommand = new AddPluginToAgentCommand
        {
            AgentId = agent.Id,
            PluginId = plugin.Id,
            UserId = Guid.NewGuid(),
            ExecutionOrder = 1,
            CustomConfiguration = null
        };

        // Act: Add plugin to agent
        var response = await _client.PostAsJsonAsync("/api/agents/plugins/add", addPluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();

        // Verify plugin was added by querying agent plugins
        var pluginsResponse = await _client.GetAsync($"/api/agents/{agent.Id}/plugins");
        pluginsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var agentPlugins = await pluginsResponse.Content.ReadFromJsonAsync<List<AgentPluginDto>>();
        agentPlugins.Should().NotBeNull();
        agentPlugins!.Should().Contain(ap => ap.PluginId == plugin.Id);
    }

    [Fact]
    public async Task GetAgentPlugins_WithValidAgentId_ShouldReturnPluginList()
    {
        // Arrange: Create agent with plugins
        var agent = await CreateTestAgentAsync();
        var plugin1 = await CreateTestPluginAsync("TestPlugin1", PluginType.Tool);
        var plugin2 = await CreateTestPluginAsync("TestPlugin2", PluginType.Function);

        // Add plugins to agent
        await AddPluginToAgentAsync(agent.Id, plugin1.Id, 1);
        await AddPluginToAgentAsync(agent.Id, plugin2.Id, 2);

        // Act: Get agent plugins
        var response = await _client.GetAsync($"/api/agents/{agent.Id}/plugins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var agentPlugins = await response.Content.ReadFromJsonAsync<List<AgentPluginDto>>();

        agentPlugins.Should().NotBeNull();
        agentPlugins!.Should().HaveCountGreaterOrEqualTo(2);
        agentPlugins.Should().Contain(ap => ap.PluginId == plugin1.Id);
        agentPlugins.Should().Contain(ap => ap.PluginId == plugin2.Id);
    }

    [Fact]
    public async Task GetAgentPlugins_WithEnabledOnlyFilter_ShouldReturnOnlyEnabledPlugins()
    {
        // Arrange: Create agent with enabled and disabled plugins
        var agent = await CreateTestAgentAsync();
        var enabledPlugin = await CreateTestPluginAsync("EnabledPlugin", PluginType.Tool, isEnabled: true);
        var disabledPlugin = await CreateTestPluginAsync("DisabledPlugin", PluginType.Function, isEnabled: false);

        await AddPluginToAgentAsync(agent.Id, enabledPlugin.Id, 1);
        await AddPluginToAgentAsync(agent.Id, disabledPlugin.Id, 2);

        // Act: Get only enabled plugins
        var response = await _client.GetAsync($"/api/agents/{agent.Id}/plugins?enabledOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var agentPlugins = await response.Content.ReadFromJsonAsync<List<AgentPluginDto>>();

        agentPlugins.Should().NotBeNull();
        // All returned plugins should be enabled
        agentPlugins!.Should().OnlyContain(ap => ap.Plugin.IsEnabled);
    }

    [Fact]
    public async Task RemovePluginFromAgent_WithValidData_ShouldSucceed()
    {
        // Arrange: Create agent with plugin
        var agent = await CreateTestAgentAsync();
        var plugin = await CreateTestPluginAsync();
        await AddPluginToAgentAsync(agent.Id, plugin.Id, 1);

        var removePluginCommand = new RemovePluginFromAgentCommand
        {
            AgentId = agent.Id,
            PluginId = plugin.Id
        };

        // Act: Remove plugin from agent
        var response = await _client.PostAsJsonAsync("/api/agents/plugins/remove", removePluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();

        // Verify plugin was removed
        var pluginsResponse = await _client.GetAsync($"/api/agents/{agent.Id}/plugins");
        var agentPlugins = await pluginsResponse.Content.ReadFromJsonAsync<List<AgentPluginDto>>();
        agentPlugins!.Should().NotContain(ap => ap.PluginId == plugin.Id);
    }

    [Fact]
    public async Task UpdateAgentPlugin_WithValidData_ShouldSucceed()
    {
        // Arrange: Create agent with plugin
        var agent = await CreateTestAgentAsync();
        var plugin = await CreateTestPluginAsync();
        await AddPluginToAgentAsync(agent.Id, plugin.Id, 1);

        var updatePluginCommand = new UpdateAgentPluginCommand
        {
            AgentId = agent.Id,
            PluginId = plugin.Id,
            IsEnabled = false,
            ExecutionOrder = 10,
            CustomConfiguration = "{\"timeout\": 30}"
        };

        // Act: Update plugin configuration
        var response = await _client.PutAsJsonAsync("/api/agents/plugins/update", updatePluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();

        // Verify plugin configuration was updated
        var pluginsResponse = await _client.GetAsync($"/api/agents/{agent.Id}/plugins");
        var agentPlugins = await pluginsResponse.Content.ReadFromJsonAsync<List<AgentPluginDto>>();
        var updatedPlugin = agentPlugins!.FirstOrDefault(ap => ap.PluginId == plugin.Id);

        updatedPlugin.Should().NotBeNull();
        updatedPlugin!.IsEnabled.Should().BeFalse();
        updatedPlugin.ExecutionOrder.Should().Be(10);
        updatedPlugin.CustomConfiguration.Should().Be("{\"timeout\": 30}");
    }

    [Fact]
    public async Task AddPluginToAgent_WithNonexistentAgent_ShouldReturnNotFound()
    {
        // Arrange
        var nonexistentAgentId = Guid.NewGuid();
        var plugin = await CreateTestPluginAsync();

        var addPluginCommand = new AddPluginToAgentCommand
        {
            AgentId = nonexistentAgentId,
            PluginId = plugin.Id,
            UserId = Guid.NewGuid(),
            ExecutionOrder = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/agents/plugins/add", addPluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPluginToAgent_WithNonexistentPlugin_ShouldReturnNotFound()
    {
        // Arrange
        var agent = await CreateTestAgentAsync();
        var nonexistentPluginId = Guid.NewGuid();

        var addPluginCommand = new AddPluginToAgentCommand
        {
            AgentId = agent.Id,
            PluginId = nonexistentPluginId,
            UserId = Guid.NewGuid(),
            ExecutionOrder = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/agents/plugins/add", addPluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPluginToAgent_DuplicatePlugin_ShouldReturnBadRequest()
    {
        // Arrange: Create agent with plugin already added
        var agent = await CreateTestAgentAsync();
        var plugin = await CreateTestPluginAsync();
        await AddPluginToAgentAsync(agent.Id, plugin.Id, 1);

        // Try to add the same plugin again
        var addPluginCommand = new AddPluginToAgentCommand
        {
            AgentId = agent.Id,
            PluginId = plugin.Id,
            UserId = Guid.NewGuid(),
            ExecutionOrder = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/agents/plugins/add", addPluginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #region Helper Methods

    private async Task<AgentDto> CreateTestAgentAsync()
    {
        var createCommand = new CreateAgentCommand
        {
            Name = "Test Agent for Plugins",
            Description = "Testing plugin functionality",
            Model = "gpt-4o",
            Instructions = "You are a test assistant",
            UserId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/agents", createCommand);
        response.EnsureSuccessStatusCode();

        var agent = await response.Content.ReadFromJsonAsync<AgentDto>();
        return agent!;
    }

    private async Task<Plugin> CreateTestPluginAsync(
        string? name = null,
        PluginType? type = null,
        bool isEnabled = true)
    {
        // Create plugin directly in database through factory's DbContext
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AIAgentPlatform.Infrastructure.Data.ApplicationDbContext>();

        var plugin = Plugin.Create(
            name ?? $"TestPlugin_{Guid.NewGuid():N}",
            type ?? PluginType.Tool,
            "1.0.0",
            "Test plugin description",
            "{\"default\": \"config\"}",
            "Test Author"
        );

        if (!isEnabled)
        {
            plugin.Disable();
        }

        dbContext.Plugins.Add(plugin);
        await dbContext.SaveChangesAsync();

        return plugin;
    }

    private async Task AddPluginToAgentAsync(Guid agentId, Guid pluginId, int executionOrder)
    {
        var addPluginCommand = new AddPluginToAgentCommand
        {
            AgentId = agentId,
            PluginId = pluginId,
            UserId = Guid.NewGuid(),
            ExecutionOrder = executionOrder
        };

        var response = await _client.PostAsJsonAsync("/api/agents/plugins/add", addPluginCommand);
        response.EnsureSuccessStatusCode();
    }

    #endregion
}
