using AIAgentPlatform.Application.Queries.Plugins.GetPlugin;
using AIAgentPlatform.Application.Queries.Plugins.GetPlugins;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Queries;

public sealed class GetPluginQueryTests
{
    private readonly Mock<IPluginRepository> _mockRepository;

    public GetPluginQueryTests()
    {
        _mockRepository = new Mock<IPluginRepository>();
    }

    [Fact]
    public async Task GetPlugin_WithValidId_ShouldReturnPlugin()
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

        var handler = new GetPluginQueryHandler(_mockRepository.Object);
        var query = new GetPluginQuery(plugin.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(plugin.Id);
        result.PluginId.Should().Be("TestPlugin");
    }

    [Fact]
    public async Task GetPlugin_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null);

        var handler = new GetPluginQueryHandler(_mockRepository.Object);
        var query = new GetPluginQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPlugins_ShouldReturnFilteredPlugins()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plugins = new List<Plugin>
        {
            Plugin.Create(userId, "Plugin1", "Plugin 1", "1.0.0", PluginMetadata.Empty),
            Plugin.Create(userId, "Plugin2", "Plugin 2", "1.0.0", PluginMetadata.Empty)
        };

        _mockRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<Guid?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugins);

        var handler = new GetPluginsQueryHandler(_mockRepository.Object);
        var query = new GetPluginsQuery
        {
            UserId = userId,
            Take = 50
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPlugins_WithExcessiveTake_ShouldLimitTo100()
    {
        // Arrange
        var handler = new GetPluginsQueryHandler(_mockRepository.Object);
        var query = new GetPluginsQuery
        {
            Take = 500 // Request too many
        };

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            x => x.GetAllAsync(
                It.IsAny<Guid?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                100, // Should be capped at 100
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
