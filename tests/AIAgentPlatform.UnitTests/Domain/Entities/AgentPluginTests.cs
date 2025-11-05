using AIAgentPlatform.Domain.Entities;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public sealed class AgentPluginTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAgentPlugin()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();
        var addedBy = Guid.NewGuid();
        var executionOrder = 1;
        var customConfiguration = "{\"timeout\":30}";

        // Act
        var agentPlugin = AgentPlugin.Create(agentId, pluginId, addedBy, executionOrder, customConfiguration);

        // Assert
        agentPlugin.Should().NotBeNull();
        agentPlugin.AgentId.Should().Be(agentId);
        agentPlugin.PluginId.Should().Be(pluginId);
        agentPlugin.AddedBy.Should().Be(addedBy);
        agentPlugin.ExecutionOrder.Should().Be(executionOrder);
        agentPlugin.CustomConfiguration.Should().Be(customConfiguration);
        agentPlugin.IsEnabled.Should().BeTrue();
        agentPlugin.AddedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithMinimalParameters_ShouldCreateAgentPlugin()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();
        var addedBy = Guid.NewGuid();

        // Act
        var agentPlugin = AgentPlugin.Create(agentId, pluginId, addedBy);

        // Assert
        agentPlugin.Should().NotBeNull();
        agentPlugin.ExecutionOrder.Should().Be(0);
        agentPlugin.CustomConfiguration.Should().BeNull();
        agentPlugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeExecutionOrder_ShouldThrowArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();
        var addedBy = Guid.NewGuid();

        // Act & Assert
        var action = () => AgentPlugin.Create(agentId, pluginId, addedBy, -1);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Execution order cannot be negative*");
    }

    [Fact]
    public void UpdateConfiguration_WithValidParameters_ShouldUpdateConfiguration()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var newOrder = 5;
        var newConfiguration = "{\"maxRetries\":3}";

        // Act
        agentPlugin.UpdateConfiguration(false, newOrder, newConfiguration);

        // Assert
        agentPlugin.IsEnabled.Should().BeFalse();
        agentPlugin.ExecutionOrder.Should().Be(newOrder);
        agentPlugin.CustomConfiguration.Should().Be(newConfiguration);
    }

    [Fact]
    public void UpdateConfiguration_WithNegativeExecutionOrder_ShouldThrowArgumentException()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var action = () => agentPlugin.UpdateConfiguration(true, -1, null);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Execution order cannot be negative*");
    }

    [Fact]
    public void UpdateConfiguration_ToEnableDisabledPlugin_ShouldUpdateCorrectly()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agentPlugin.UpdateConfiguration(false, 0, null);

        // Act
        agentPlugin.UpdateConfiguration(true, 0, null);

        // Assert
        agentPlugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void UpdateConfiguration_ToDisableEnabledPlugin_ShouldUpdateCorrectly()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        agentPlugin.UpdateConfiguration(false, 0, null);

        // Assert
        agentPlugin.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void UpdateConfiguration_WithNullConfiguration_ShouldAcceptNullValue()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, "{\"test\":true}");

        // Act
        agentPlugin.UpdateConfiguration(true, 0, null);

        // Assert
        agentPlugin.CustomConfiguration.Should().BeNull();
    }

    [Fact]
    public void UpdateConfiguration_MultipleUpdates_ShouldRetainLatestValues()
    {
        // Arrange
        var agentPlugin = AgentPlugin.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        agentPlugin.UpdateConfiguration(false, 1, "{\"v1\":true}");
        agentPlugin.UpdateConfiguration(true, 2, "{\"v2\":true}");
        agentPlugin.UpdateConfiguration(false, 3, "{\"v3\":true}");

        // Assert
        agentPlugin.IsEnabled.Should().BeFalse();
        agentPlugin.ExecutionOrder.Should().Be(3);
        agentPlugin.CustomConfiguration.Should().Be("{\"v3\":true}");
    }
}
