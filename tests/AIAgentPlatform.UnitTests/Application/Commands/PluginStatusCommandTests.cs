using AIAgentPlatform.Application.Commands.Plugins.ActivatePlugin;
using AIAgentPlatform.Application.Commands.Plugins.DeactivatePlugin;
using AIAgentPlatform.Application.Commands.Plugins.UpdatePlugin;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class PluginStatusCommandTests
{
    private readonly Mock<IPluginRepository> _mockRepository;

    public PluginStatusCommandTests()
    {
        _mockRepository = new Mock<IPluginRepository>();
    }

    [Fact]
    public async Task ActivatePlugin_WithValidId_ShouldActivatePlugin()
    {
        // Arrange
        var plugin = Plugin.Create(
            Guid.NewGuid(),
            "TestPlugin",
            "Test Plugin",
            "1.0.0",
            PluginMetadata.Empty);

        plugin.Deactivate(); // Start as inactive

        _mockRepository
            .Setup(x => x.GetByIdAsync(plugin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugin);

        var handler = new ActivatePluginCommandHandler(_mockRepository.Object);
        var command = new ActivatePluginCommand(plugin.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        plugin.Status.IsActive.Should().BeTrue();
        _mockRepository.Verify(x => x.UpdateAsync(plugin, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivatePlugin_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null);

        var handler = new ActivatePluginCommandHandler(_mockRepository.Object);
        var command = new ActivatePluginCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeactivatePlugin_WithValidId_ShouldDeactivatePlugin()
    {
        // Arrange
        var plugin = Plugin.Create(
            Guid.NewGuid(),
            "TestPlugin",
            "Test Plugin",
            "1.0.0",
            PluginMetadata.Empty);

        plugin.Activate(); // Start as active

        _mockRepository
            .Setup(x => x.GetByIdAsync(plugin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugin);

        var handler = new DeactivatePluginCommandHandler(_mockRepository.Object);
        var command = new DeactivatePluginCommand(plugin.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        plugin.Status.IsInactive.Should().BeTrue();
        _mockRepository.Verify(x => x.UpdateAsync(plugin, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlugin_WithValidData_ShouldUpdatePlugin()
    {
        // Arrange
        var plugin = Plugin.Create(
            Guid.NewGuid(),
            "TestPlugin",
            "Test Plugin",
            "1.0.0",
            PluginMetadata.Empty);

        _mockRepository
            .Setup(x => x.GetByIdAsync(plugin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugin);

        var handler = new UpdatePluginCommandHandler(_mockRepository.Object);
        var command = new UpdatePluginCommand
        {
            Id = plugin.Id,
            Name = "Updated Name",
            Description = "Updated Description",
            Category = "Updated Category"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        plugin.Name.Should().Be("Updated Name");
        plugin.Description.Should().Be("Updated Description");
        plugin.Category.Should().Be("Updated Category");
        _mockRepository.Verify(x => x.UpdateAsync(plugin, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void UpdatePluginValidator_WithNoFields_ShouldFail()
    {
        // Arrange
        var validator = new UpdatePluginCommandValidator();
        var command = new UpdatePluginCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdatePluginValidator_WithAtLeastOneField_ShouldPass()
    {
        // Arrange
        var validator = new UpdatePluginCommandValidator();
        var command = new UpdatePluginCommand
        {
            Id = Guid.NewGuid(),
            Name = "New Name"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
