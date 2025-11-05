using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public sealed class AgentVersionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAgentVersion()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = "v1.0";
        var changeDescription = "Initial version";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "{\"name\":\"Test\",\"model\":\"gpt-4o\"}";
        var createdBy = Guid.NewGuid();

        // Act
        var agentVersion = AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy, true);

        // Assert
        agentVersion.Should().NotBeNull();
        agentVersion.AgentId.Should().Be(agentId);
        agentVersion.Version.Should().Be(version);
        agentVersion.ChangeDescription.Should().Be(changeDescription);
        agentVersion.ChangeType.Should().Be(changeType);
        agentVersion.ConfigurationSnapshot.Should().Be(configurationSnapshot);
        agentVersion.CreatedBy.Should().Be(createdBy);
        agentVersion.IsCurrent.Should().BeTrue();
        agentVersion.RolledBackAt.Should().BeNull();
        agentVersion.RolledBackBy.Should().BeNull();
    }

    [Fact]
    public void Create_WithIsCurrentFalse_ShouldCreateNonCurrentVersion()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = "v1.0";
        var changeDescription = "Initial version";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "{\"name\":\"Test\"}";
        var createdBy = Guid.NewGuid();

        // Act
        var agentVersion = AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy, false);

        // Assert
        agentVersion.IsCurrent.Should().BeFalse();
    }

    [Fact]
    public void Create_WithEmptyVersion_ShouldThrowArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = "";
        var changeDescription = "Initial version";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "{\"name\":\"Test\"}";
        var createdBy = Guid.NewGuid();

        // Act & Assert
        var action = () => AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Version cannot be empty*");
    }

    [Fact]
    public void Create_WithVersionTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = new string('v', 21);
        var changeDescription = "Initial version";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "{\"name\":\"Test\"}";
        var createdBy = Guid.NewGuid();

        // Act & Assert
        var action = () => AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Version cannot exceed 20 characters*");
    }

    [Fact]
    public void Create_WithEmptyChangeDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = "v1.0";
        var changeDescription = "";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "{\"name\":\"Test\"}";
        var createdBy = Guid.NewGuid();

        // Act & Assert
        var action = () => AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Change description cannot be empty*");
    }

    [Fact]
    public void Create_WithEmptyConfigurationSnapshot_ShouldThrowArgumentException()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var version = "v1.0";
        var changeDescription = "Test";
        var changeType = VersionChangeType.Major;
        var configurationSnapshot = "";
        var createdBy = Guid.NewGuid();

        // Act & Assert
        var action = () => AgentVersion.Create(agentId, version, changeDescription, changeType, configurationSnapshot, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Configuration snapshot cannot be empty*");
    }

    [Fact]
    public void MarkAsCurrent_ShouldSetIsCurrentToTrue()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), false);

        // Act
        agentVersion.MarkAsCurrent();

        // Assert
        agentVersion.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void MarkAsNotCurrent_ShouldSetIsCurrentToFalse()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), true);

        // Act
        agentVersion.MarkAsNotCurrent();

        // Assert
        agentVersion.IsCurrent.Should().BeFalse();
    }

    [Fact]
    public void RecordRollback_WithValidUserId_ShouldRecordRollbackInfo()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), false);
        var rolledBackBy = Guid.NewGuid();

        // Act
        agentVersion.RecordRollback(rolledBackBy);

        // Assert
        agentVersion.RolledBackBy.Should().Be(rolledBackBy);
        agentVersion.RolledBackAt.Should().NotBeNull();
        agentVersion.RolledBackAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RecordRollback_WhenAlreadyRolledBack_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), false);
        var firstUser = Guid.NewGuid();
        var secondUser = Guid.NewGuid();
        agentVersion.RecordRollback(firstUser);

        // Act & Assert
        var action = () => agentVersion.RecordRollback(secondUser);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Version has already been rolled back*");
    }

    [Fact]
    public void MarkAsCurrent_WhenAlreadyCurrent_ShouldRemainCurrent()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), true);

        // Act
        agentVersion.MarkAsCurrent();

        // Assert
        agentVersion.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void MarkAsNotCurrent_WhenAlreadyNotCurrent_ShouldRemainNotCurrent()
    {
        // Arrange
        var agentVersion = AgentVersion.Create(Guid.NewGuid(), "v1.0", "Test", VersionChangeType.Major, "{}", Guid.NewGuid(), false);

        // Act
        agentVersion.MarkAsNotCurrent();

        // Assert
        agentVersion.IsCurrent.Should().BeFalse();
    }
}
