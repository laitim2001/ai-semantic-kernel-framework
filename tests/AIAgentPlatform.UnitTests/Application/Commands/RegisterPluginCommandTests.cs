using AIAgentPlatform.Application.Commands.Plugins.RegisterPlugin;
using AIAgentPlatform.Application.Interfaces;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class RegisterPluginCommandTests
{
    private readonly Mock<IPluginRepository> _mockRepository;
    private readonly Mock<IPluginMetadataExtractor> _mockExtractor;
    private readonly RegisterPluginCommandHandler _handler;
    private readonly RegisterPluginCommandValidator _validator;

    public RegisterPluginCommandTests()
    {
        _mockRepository = new Mock<IPluginRepository>();
        _mockExtractor = new Mock<IPluginMetadataExtractor>();
        _handler = new RegisterPluginCommandHandler(_mockRepository.Object, _mockExtractor.Object);
        _validator = new RegisterPluginCommandValidator();
    }

    [Fact]
    public async Task Handle_WithAssemblyPath_ShouldExtractAndRegisterPlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterPluginCommand
        {
            UserId = userId,
            AssemblyPath = "test-plugin.dll"
        };

        var metadataResult = PluginMetadataResult.Success(
            "TestPlugin",
            "Test Plugin",
            "1.0.0",
            PluginMetadata.Empty);

        _mockExtractor
            .Setup(x => x.ExtractFromAssemblyAsync(command.AssemblyPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(metadataResult);

        _mockRepository
            .Setup(x => x.GetByPluginIdAsync("TestPlugin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null);

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin p, CancellationToken _) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PluginId.Should().Be("TestPlugin");
        result.Name.Should().Be("Test Plugin");
        result.Version.Should().Be("1.0.0");

        _mockExtractor.Verify(x => x.ExtractFromAssemblyAsync(command.AssemblyPath, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithManualMetadata_ShouldRegisterPlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterPluginCommand
        {
            UserId = userId,
            PluginId = "ManualPlugin",
            Name = "Manual Plugin",
            Version = "1.0.0",
            Description = "Test description",
            Category = "Test"
        };

        _mockRepository
            .Setup(x => x.GetByPluginIdAsync("ManualPlugin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null);

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin p, CancellationToken _) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PluginId.Should().Be("ManualPlugin");
        result.Name.Should().Be("Manual Plugin");
        result.Version.Should().Be("1.0.0");
        result.Description.Should().Be("Test description");

        _mockExtractor.Verify(x => x.ExtractFromAssemblyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingPluginId_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterPluginCommand
        {
            UserId = userId,
            PluginId = "ExistingPlugin",
            Name = "Existing Plugin",
            Version = "1.0.0"
        };

        var existingPlugin = Plugin.Create(
            userId,
            "ExistingPlugin",
            "Existing Plugin",
            "1.0.0",
            PluginMetadata.Empty);

        _mockRepository
            .Setup(x => x.GetByPluginIdAsync("ExistingPlugin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlugin);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithAutoActivate_ShouldActivatePlugin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterPluginCommand
        {
            UserId = userId,
            PluginId = "AutoActivePlugin",
            Name = "Auto Active Plugin",
            Version = "1.0.0",
            AutoActivate = true
        };

        _mockRepository
            .Setup(x => x.GetByPluginIdAsync("AutoActivePlugin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null);

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Plugin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin p, CancellationToken _) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Validator_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var command = new RegisterPluginCommand
        {
            UserId = Guid.Empty,
            PluginId = "Test",
            Name = "Test",
            Version = "1.0.0"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.UserId));
    }

    [Fact]
    public void Validator_WithNoAssemblyPathOrMetadata_ShouldFail()
    {
        // Arrange
        var command = new RegisterPluginCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_WithValidAssemblyPath_ShouldPass()
    {
        // Arrange
        var command = new RegisterPluginCommand
        {
            UserId = Guid.NewGuid(),
            AssemblyPath = "plugin.dll"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_WithValidManualMetadata_ShouldPass()
    {
        // Arrange
        var command = new RegisterPluginCommand
        {
            UserId = Guid.NewGuid(),
            PluginId = "TestPlugin",
            Name = "Test Plugin",
            Version = "1.0.0"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_WithInvalidVersion_ShouldFail()
    {
        // Arrange
        var command = new RegisterPluginCommand
        {
            UserId = Guid.NewGuid(),
            PluginId = "Test",
            Name = "Test",
            Version = "invalid-version"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Version));
    }
}
