using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public sealed class PluginTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePlugin()
    {
        // Arrange
        var name = "Weather Plugin";
        var description = "Provides weather information";
        var type = PluginType.Tool;
        var version = "1.0.0";
        var configuration = "{\"apiKey\":\"test\"}";
        var author = "John Doe";

        // Act
        var plugin = Plugin.Create(name, type, version, description, configuration, author);

        // Assert
        plugin.Should().NotBeNull();
        plugin.Name.Should().Be(name);
        plugin.Description.Should().Be(description);
        plugin.Type.Should().Be(type);
        plugin.Version.Should().Be(version);
        plugin.Configuration.Should().Be(configuration);
        plugin.Author.Should().Be(author);
        plugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Create_WithMinimalParameters_ShouldCreatePlugin()
    {
        // Arrange
        var name = "Simple Plugin";
        var type = PluginType.Function;
        var version = "1.0.0";

        // Act
        var plugin = Plugin.Create(name, type, version);

        // Assert
        plugin.Should().NotBeNull();
        plugin.Name.Should().Be(name);
        plugin.Type.Should().Be(type);
        plugin.Version.Should().Be(version);
        plugin.Description.Should().BeNull();
        plugin.Configuration.Should().BeNull();
        plugin.Author.Should().BeNull();
        plugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "";
        var type = PluginType.Tool;
        var version = "1.0.0";

        // Act & Assert
        var action = () => Plugin.Create(name, type, version);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Plugin name cannot be empty*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var name = new string('a', 101);
        var type = PluginType.Tool;
        var version = "1.0.0";

        // Act & Assert
        var action = () => Plugin.Create(name, type, version);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Plugin name cannot exceed 100 characters*");
    }

    [Fact]
    public void Create_WithEmptyVersion_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Test Plugin";
        var type = PluginType.Tool;
        var version = "";

        // Act & Assert
        var action = () => Plugin.Create(name, type, version);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Plugin version cannot be empty*");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdatePlugin()
    {
        // Arrange
        var plugin = Plugin.Create("Original", PluginType.Tool, "1.0.0");
        var newName = "Updated Plugin";
        var newDescription = "Updated description";
        var newConfiguration = "{\"updated\":true}";

        // Act
        plugin.Update(newName, newDescription, newConfiguration);

        // Assert
        plugin.Name.Should().Be(newName);
        plugin.Description.Should().Be(newDescription);
        plugin.Configuration.Should().Be(newConfiguration);
    }

    [Fact]
    public void Update_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var plugin = Plugin.Create("Original", PluginType.Tool, "1.0.0");

        // Act & Assert
        var action = () => plugin.Update("", "desc", null);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Plugin name cannot be empty*");
    }

    [Fact]
    public void Enable_ShouldSetIsEnabledToTrue()
    {
        // Arrange
        var plugin = Plugin.Create("Test", PluginType.Tool, "1.0.0");
        plugin.Disable();

        // Act
        plugin.Enable();

        // Assert
        plugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Disable_ShouldSetIsEnabledToFalse()
    {
        // Arrange
        var plugin = Plugin.Create("Test", PluginType.Tool, "1.0.0");

        // Act
        plugin.Disable();

        // Assert
        plugin.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void Enable_WhenAlreadyEnabled_ShouldRemainEnabled()
    {
        // Arrange
        var plugin = Plugin.Create("Test", PluginType.Tool, "1.0.0");
        plugin.Enable();

        // Act
        plugin.Enable();

        // Assert
        plugin.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Disable_WhenAlreadyDisabled_ShouldRemainDisabled()
    {
        // Arrange
        var plugin = Plugin.Create("Test", PluginType.Tool, "1.0.0");
        plugin.Disable();

        // Act
        plugin.Disable();

        // Assert
        plugin.IsEnabled.Should().BeFalse();
    }
}
