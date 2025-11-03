using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.Entities;

public sealed class AgentTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Test Agent";
        var instructions = "You are a helpful assistant";
        var model = LLMModel.GPT4o;

        // Act
        var agent = Agent.Create(userId, name, instructions, model);

        // Assert
        agent.Should().NotBeNull();
        agent.UserId.Should().Be(userId);
        agent.Name.Should().Be(name);
        agent.Instructions.Should().Be(instructions);
        agent.Model.Should().Be(model);
        agent.Temperature.Should().Be(0.7m);
        agent.MaxTokens.Should().Be(4096);
        agent.Status.Should().Be(AgentStatus.Active);
        agent.Id.Should().NotBeEmpty();
        agent.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "";
        var instructions = "You are a helpful assistant";
        var model = LLMModel.GPT4o;

        // Act & Assert
        var action = () => Agent.Create(userId, name, instructions, model);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = new string('a', 101);
        var instructions = "You are a helpful assistant";
        var model = LLMModel.GPT4o;

        // Act & Assert
        var action = () => Agent.Create(userId, name, instructions, model);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 100 characters*");
    }

    [Fact]
    public void Create_WithInvalidTemperature_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Test Agent";
        var instructions = "You are a helpful assistant";
        var model = LLMModel.GPT4o;
        var temperature = 3m;

        // Act & Assert
        var action = () => Agent.Create(userId, name, instructions, model, temperature: temperature);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Temperature must be between 0 and 2*");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Original Name", "Original instructions", LLMModel.GPT4o);
        var newName = "Updated Name";
        var newDescription = "Updated description";
        var newInstructions = "Updated instructions";

        // Act
        agent.Update(newName, newDescription, newInstructions);

        // Assert
        agent.Name.Should().Be(newName);
        agent.Description.Should().Be(newDescription);
        agent.Instructions.Should().Be(newInstructions);
    }

    [Fact]
    public void UpdateModelConfiguration_WithValidParameters_ShouldUpdateModel()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Test Agent", "Instructions", LLMModel.GPT4o);
        var newModel = LLMModel.GPT4oMini;
        var newTemperature = 0.5m;
        var newMaxTokens = 2000;

        // Act
        agent.UpdateModelConfiguration(newModel, newTemperature, newMaxTokens);

        // Assert
        agent.Model.Should().Be(newModel);
        agent.Temperature.Should().Be(newTemperature);
        agent.MaxTokens.Should().Be(newMaxTokens);
    }

    [Fact]
    public void Activate_WhenPaused_ShouldActivateAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Test Agent", "Instructions", LLMModel.GPT4o);
        agent.Pause();

        // Act
        agent.Activate();

        // Assert
        agent.Status.Should().Be(AgentStatus.Active);
        agent.Status.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Pause_WhenActive_ShouldPauseAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Test Agent", "Instructions", LLMModel.GPT4o);

        // Act
        agent.Pause();

        // Assert
        agent.Status.Should().Be(AgentStatus.Paused);
        agent.Status.IsPaused.Should().BeTrue();
    }

    [Fact]
    public void Archive_WhenActive_ShouldArchiveAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Test Agent", "Instructions", LLMModel.GPT4o);

        // Act
        agent.Archive();

        // Assert
        agent.Status.Should().Be(AgentStatus.Archived);
        agent.Status.IsArchived.Should().BeTrue();
    }
}
