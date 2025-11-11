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
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = "Weather Plugin";
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act
        var plugin = Plugin.Create(userId, pluginId, name, version, metadata);

        // Assert
        plugin.Should().NotBeNull();
        plugin.UserId.Should().Be(userId);
        plugin.PluginId.Should().Be(pluginId);
        plugin.Name.Should().Be(name);
        plugin.Version.Should().Be(version);
        plugin.Metadata.Should().Be(metadata);
        plugin.Status.Should().Be(PluginStatus.Active);
        plugin.Id.Should().NotBeEmpty();
        plugin.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldCreatePluginWithOptionalData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = "Weather Plugin";
        var version = "1.0.0";
        var description = "Get weather information";
        var category = "Utilities";
        var assemblyPath = "/plugins/weather.dll";
        var metadata = PluginMetadata.Empty;

        // Act
        var plugin = Plugin.Create(
            userId,
            pluginId,
            name,
            version,
            metadata,
            description: description,
            category: category,
            assemblyPath: assemblyPath);

        // Assert
        plugin.Description.Should().Be(description);
        plugin.Category.Should().Be(category);
        plugin.AssemblyPath.Should().Be(assemblyPath);
    }

    [Fact]
    public void Create_WithEmptyPluginId_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "";
        var name = "Weather Plugin";
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Plugin ID cannot be empty*");
    }

    [Fact]
    public void Create_WithInvalidPluginIdCharacters_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "Weather@Plugin!";
        var name = "Weather Plugin";
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*can only contain letters, numbers, underscores, and hyphens*");
    }

    [Fact]
    public void Create_WithPluginIdTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = new string('a', 101);
        var name = "Weather Plugin";
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 100 characters*");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = "";
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = new string('a', 201);
        var version = "1.0.0";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Fact]
    public void Create_WithInvalidVersionFormat_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = "Weather Plugin";
        var version = "invalid-version";
        var metadata = PluginMetadata.Empty;

        // Act & Assert
        var action = () => Plugin.Create(userId, pluginId, name, version, metadata);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*must follow semantic versioning*");
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("2.5.3")]
    [InlineData("10.20.30")]
    [InlineData("1.0.0-alpha")]
    [InlineData("1.0.0-beta1")]
    public void Create_WithValidVersionFormats_ShouldCreatePlugin(string version)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pluginId = "WeatherPlugin";
        var name = "Weather Plugin";
        var metadata = PluginMetadata.Empty;

        // Act
        var plugin = Plugin.Create(userId, pluginId, name, version, metadata);

        // Assert
        plugin.Version.Should().Be(version);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdatePlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Original Name", "1.0.0", PluginMetadata.Empty);
        var newName = "Updated Name";
        var newDescription = "Updated description";
        var newCategory = "Updated category";

        // Act
        plugin.Update(newName, newDescription, newCategory);

        // Assert
        plugin.Name.Should().Be(newName);
        plugin.Description.Should().Be(newDescription);
        plugin.Category.Should().Be(newCategory);
    }

    [Fact]
    public void UpdateVersionAndMetadata_WithValidParameters_ShouldUpdateVersionAndMetadata()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        var newVersion = "2.0.0";
        var newMetadata = PluginMetadata.Create(new List<PluginFunction>());

        // Act
        plugin.UpdateVersionAndMetadata(newVersion, newMetadata);

        // Assert
        plugin.Version.Should().Be(newVersion);
        plugin.Metadata.Should().Be(newMetadata);
    }

    [Fact]
    public void UpdateAssemblyInfo_WithValidParameters_ShouldUpdateAssemblyInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        var assemblyPath = "/plugins/weather-v2.dll";
        var assemblyFullName = "WeatherPlugin, Version=2.0.0.0";

        // Act
        plugin.UpdateAssemblyInfo(assemblyPath, assemblyFullName);

        // Assert
        plugin.AssemblyPath.Should().Be(assemblyPath);
        plugin.AssemblyFullName.Should().Be(assemblyFullName);
    }

    [Fact]
    public void Activate_WhenInactive_ShouldActivatePlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        plugin.Deactivate();

        // Act
        plugin.Activate();

        // Assert
        plugin.Status.Should().Be(PluginStatus.Active);
        plugin.Status.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivatePlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);

        // Act
        plugin.Deactivate();

        // Assert
        plugin.Status.Should().Be(PluginStatus.Inactive);
        plugin.Status.IsInactive.Should().BeTrue();
    }

    [Fact]
    public void MarkAsFailed_ShouldSetStatusToFailed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);

        // Act
        plugin.MarkAsFailed();

        // Assert
        plugin.Status.Should().Be(PluginStatus.Failed);
        plugin.Status.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void MarkAsFailed_WithErrorMessage_ShouldSetStatusAndUpdateDescription()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        var errorMessage = "Failed to load assembly";

        // Act
        plugin.MarkAsFailed(errorMessage);

        // Assert
        plugin.Status.Should().Be(PluginStatus.Failed);
        plugin.Description.Should().Be(errorMessage);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldNotChangeStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        var originalUpdatedAt = plugin.UpdatedAt;

        // Act
        plugin.Activate();

        // Assert
        plugin.Status.Should().Be(PluginStatus.Active);
        plugin.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldNotChangeStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugin = Plugin.Create(userId, "WeatherPlugin", "Weather Plugin", "1.0.0", PluginMetadata.Empty);
        plugin.Deactivate();
        var originalUpdatedAt = plugin.UpdatedAt;

        // Act
        plugin.Deactivate();

        // Assert
        plugin.Status.Should().Be(PluginStatus.Inactive);
        plugin.UpdatedAt.Should().Be(originalUpdatedAt);
    }
}
