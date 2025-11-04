using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class BatchOperationsCommandTests
{
    private readonly Mock<IAgentRepository> _mockRepository;

    public BatchOperationsCommandTests()
    {
        _mockRepository = new Mock<IAgentRepository>();
    }

    #region BatchActivateAgentsCommand Tests

    [Fact]
    public async Task BatchActivate_WithValidAgents_ShouldActivateAll()
    {
        // Arrange
        var agent1 = Agent.Create(Guid.NewGuid(), "Agent 1", "Test", LLMModel.GPT4o);
        var agent2 = Agent.Create(Guid.NewGuid(), "Agent 2", "Test", LLMModel.GPT4o);
        agent1.Pause();
        agent2.Pause();

        _mockRepository.Setup(x => x.GetByIdAsync(agent1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent1);
        _mockRepository.Setup(x => x.GetByIdAsync(agent2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent2);

        var command = new BatchActivateAgentsCommand
        {
            AgentIds = new List<Guid> { agent1.Id, agent2.Id }
        };
        var handler = new BatchActivateAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
        result.SuccessfulIds.Should().Contain(agent1.Id);
        result.SuccessfulIds.Should().Contain(agent2.Id);
        result.Errors.Should().BeEmpty();

        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BatchActivate_WithNonExistentAgent_ShouldReturnError()
    {
        // Arrange
        var validAgentId = Guid.NewGuid();
        var invalidAgentId = Guid.NewGuid();
        var validAgent = Agent.Create(Guid.NewGuid(), "Valid Agent", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(validAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validAgent);
        _mockRepository.Setup(x => x.GetByIdAsync(invalidAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new BatchActivateAgentsCommand
        {
            AgentIds = new List<Guid> { validAgentId, invalidAgentId }
        };
        var handler = new BatchActivateAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        result.SuccessfulIds.Should().Contain(validAgentId);
        result.Errors.Should().HaveCount(1);
        result.Errors.First().AgentId.Should().Be(invalidAgentId);
        result.Errors.First().ErrorMessage.Should().Be("Agent not found");
    }

    [Fact]
    public async Task BatchActivate_WithEmptyList_ShouldReturnEmptyResult()
    {
        // Arrange
        var command = new BatchActivateAgentsCommand { AgentIds = new List<Guid>() };
        var handler = new BatchActivateAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(0);
    }

    #endregion

    #region BatchPauseAgentsCommand Tests

    [Fact]
    public async Task BatchPause_WithValidAgents_ShouldPauseAll()
    {
        // Arrange
        var agent1 = Agent.Create(Guid.NewGuid(), "Agent 1", "Test", LLMModel.GPT4o);
        var agent2 = Agent.Create(Guid.NewGuid(), "Agent 2", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(agent1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent1);
        _mockRepository.Setup(x => x.GetByIdAsync(agent2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent2);

        var command = new BatchPauseAgentsCommand
        {
            AgentIds = new List<Guid> { agent1.Id, agent2.Id }
        };
        var handler = new BatchPauseAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
        result.SuccessfulIds.Should().Contain(agent1.Id);
        result.SuccessfulIds.Should().Contain(agent2.Id);

        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BatchPause_WithNonExistentAgent_ShouldReturnError()
    {
        // Arrange
        var validAgentId = Guid.NewGuid();
        var invalidAgentId = Guid.NewGuid();
        var validAgent = Agent.Create(Guid.NewGuid(), "Valid Agent", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(validAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validAgent);
        _mockRepository.Setup(x => x.GetByIdAsync(invalidAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new BatchPauseAgentsCommand
        {
            AgentIds = new List<Guid> { validAgentId, invalidAgentId }
        };
        var handler = new BatchPauseAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("Agent not found");
    }

    #endregion

    #region BatchArchiveAgentsCommand Tests

    [Fact]
    public async Task BatchArchive_WithValidAgents_ShouldArchiveAll()
    {
        // Arrange
        var agent1 = Agent.Create(Guid.NewGuid(), "Agent 1", "Test", LLMModel.GPT4o);
        var agent2 = Agent.Create(Guid.NewGuid(), "Agent 2", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(agent1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent1);
        _mockRepository.Setup(x => x.GetByIdAsync(agent2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent2);

        var command = new BatchArchiveAgentsCommand
        {
            AgentIds = new List<Guid> { agent1.Id, agent2.Id }
        };
        var handler = new BatchArchiveAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
        result.SuccessfulIds.Should().Contain(agent1.Id);
        result.SuccessfulIds.Should().Contain(agent2.Id);

        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BatchArchive_WithNonExistentAgent_ShouldReturnError()
    {
        // Arrange
        var validAgentId = Guid.NewGuid();
        var invalidAgentId = Guid.NewGuid();
        var validAgent = Agent.Create(Guid.NewGuid(), "Valid Agent", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(validAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validAgent);
        _mockRepository.Setup(x => x.GetByIdAsync(invalidAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new BatchArchiveAgentsCommand
        {
            AgentIds = new List<Guid> { validAgentId, invalidAgentId }
        };
        var handler = new BatchArchiveAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        result.Errors.First().ErrorMessage.Should().Be("Agent not found");
    }

    #endregion

    #region BatchDeleteAgentsCommand Tests

    [Fact]
    public async Task BatchDelete_WithValidAgents_ShouldDeleteAll()
    {
        // Arrange
        var agent1 = Agent.Create(Guid.NewGuid(), "Agent 1", "Test", LLMModel.GPT4o);
        var agent2 = Agent.Create(Guid.NewGuid(), "Agent 2", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(agent1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent1);
        _mockRepository.Setup(x => x.GetByIdAsync(agent2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent2);

        var command = new BatchDeleteAgentsCommand
        {
            AgentIds = new List<Guid> { agent1.Id, agent2.Id }
        };
        var handler = new BatchDeleteAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
        result.SuccessfulIds.Should().Contain(agent1.Id);
        result.SuccessfulIds.Should().Contain(agent2.Id);

        // Verify agents are archived (soft deleted)
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BatchDelete_WithNonExistentAgent_ShouldReturnError()
    {
        // Arrange
        var validAgentId = Guid.NewGuid();
        var invalidAgentId = Guid.NewGuid();
        var validAgent = Agent.Create(Guid.NewGuid(), "Valid Agent", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetByIdAsync(validAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validAgent);
        _mockRepository.Setup(x => x.GetByIdAsync(invalidAgentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new BatchDeleteAgentsCommand
        {
            AgentIds = new List<Guid> { validAgentId, invalidAgentId }
        };
        var handler = new BatchDeleteAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        result.Errors.First().AgentId.Should().Be(invalidAgentId);
        result.Errors.First().ErrorMessage.Should().Be("Agent not found");
    }

    [Fact]
    public async Task BatchDelete_WithEmptyList_ShouldReturnEmptyResult()
    {
        // Arrange
        var command = new BatchDeleteAgentsCommand { AgentIds = new List<Guid>() };
        var handler = new BatchDeleteAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(0);
    }

    #endregion

    #region Mixed Scenario Tests

    [Fact]
    public async Task BatchOperations_WithMixedResults_ShouldHandlePartialSuccess()
    {
        // Arrange
        var successAgent = Agent.Create(Guid.NewGuid(), "Success Agent", "Test", LLMModel.GPT4o);
        var notFoundId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(successAgent.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successAgent);
        _mockRepository.Setup(x => x.GetByIdAsync(notFoundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent?)null);

        var command = new BatchActivateAgentsCommand
        {
            AgentIds = new List<Guid> { successAgent.Id, notFoundId }
        };
        var handler = new BatchActivateAgentsCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        result.SuccessfulIds.Should().Contain(successAgent.Id);
        result.Errors.Should().HaveCount(1);
        result.Errors.First().AgentId.Should().Be(notFoundId);
    }

    #endregion
}
