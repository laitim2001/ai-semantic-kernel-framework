using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Queries;

public sealed class GetAgentsQueryTests
{
    private readonly Mock<IAgentRepository> _mockRepository;
    private readonly GetAgentsQueryHandler _handler;

    public GetAgentsQueryTests()
    {
        _mockRepository = new Mock<IAgentRepository>();
        _handler = new GetAgentsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAgents_WithNoFilters_ShouldReturnAllAgents()
    {
        // Arrange
        var agents = CreateTestAgents(3);
        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, null, null, null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents);
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var query = new GetAgentsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Agents.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetAgents_WithSearchTerm_ShouldFilterByNameOrDescription()
    {
        // Arrange
        var agents = new List<Agent>
        {
            Agent.Create(Guid.NewGuid(), "Customer Service Agent", "Handles support", LLMModel.GPT4o),
            Agent.Create(Guid.NewGuid(), "Sales Agent", "Handles sales inquiries", LLMModel.GPT4o)
        };

        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, "customer", null, null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents.Where(a => a.Name.Contains("Customer", StringComparison.OrdinalIgnoreCase)).ToList());
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, "customer", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var query = new GetAgentsQuery { SearchTerm = "customer" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(1);
        result.Agents.First().Name.Should().Contain("Customer");
    }

    [Fact]
    public async Task GetAgents_WithStatusFilter_ShouldReturnOnlyActiveAgents()
    {
        // Arrange
        var activeAgent = Agent.Create(Guid.NewGuid(), "Active Agent", "Test", LLMModel.GPT4o);
        var pausedAgent = Agent.Create(Guid.NewGuid(), "Paused Agent", "Test", LLMModel.GPT4o);
        pausedAgent.Pause();

        _mockRepository.Setup(x => x.GetAllAsync(
            null, "active", null, null, null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agent> { activeAgent });
        _mockRepository.Setup(x => x.GetCountAsync(
            null, "active", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var query = new GetAgentsQuery { Status = "active" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(1);
        result.Agents.First().Status.Should().Be("active");
    }

    [Fact]
    public async Task GetAgents_WithModelFilter_ShouldReturnOnlyMatchingModel()
    {
        // Arrange
        var gpt4Agent = Agent.Create(Guid.NewGuid(), "GPT-4 Agent", "Test", LLMModel.GPT4);
        var gpt4oAgent = Agent.Create(Guid.NewGuid(), "GPT-4o Agent", "Test", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, null, "gpt-4o", null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agent> { gpt4oAgent });
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, null, "gpt-4o", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var query = new GetAgentsQuery { Model = "gpt-4o" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(1);
        result.Agents.First().Model.Should().Be("gpt-4o");
    }

    [Fact]
    public async Task GetAgents_WithSortByName_ShouldReturnSortedAgents()
    {
        // Arrange
        var agents = new List<Agent>
        {
            Agent.Create(Guid.NewGuid(), "Zebra Agent", "Test", LLMModel.GPT4o),
            Agent.Create(Guid.NewGuid(), "Alpha Agent", "Test", LLMModel.GPT4o),
            Agent.Create(Guid.NewGuid(), "Beta Agent", "Test", LLMModel.GPT4o)
        };

        var sortedAgents = agents.OrderBy(a => a.Name).ToList();

        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, null, null, "name", "asc", 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sortedAgents);
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var query = new GetAgentsQuery { SortBy = "name", SortOrder = "asc" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(3);
        result.Agents.First().Name.Should().Be("Alpha Agent");
        result.Agents.Last().Name.Should().Be("Zebra Agent");
    }

    [Fact]
    public async Task GetAgents_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var agents = CreateTestAgents(2);

        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, null, null, null, null, 20, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents);
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var query = new GetAgentsQuery { Skip = 20, Take = 20 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(2);
        result.TotalCount.Should().Be(100);
        result.Skip.Should().Be(20);
        result.Take.Should().Be(20);
    }

    [Fact]
    public async Task GetAgents_WithTakeOver100_ShouldLimitTo100()
    {
        // Arrange
        var agents = CreateTestAgents(100);

        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, null, null, null, null, 0, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents);
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(200);

        var query = new GetAgentsQuery { Take = 150 }; // Try to take more than max

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Take.Should().Be(100); // Should be limited to 100
    }

    [Fact]
    public async Task GetAgents_WithMultipleFilters_ShouldApplyAllFilters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var agent = Agent.Create(userId, "Test Agent", "Description", LLMModel.GPT4o);

        _mockRepository.Setup(x => x.GetAllAsync(
            userId, "active", "test", "gpt-4o", null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agent> { agent });
        _mockRepository.Setup(x => x.GetCountAsync(
            userId, "active", "test", "gpt-4o", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var query = new GetAgentsQuery
        {
            UserId = userId,
            Status = "active",
            SearchTerm = "test",
            Model = "gpt-4o"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAgents_WithNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync(
            null, null, "nonexistent", null, null, null, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Agent>());
        _mockRepository.Setup(x => x.GetCountAsync(
            null, null, "nonexistent", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetAgentsQuery { SearchTerm = "nonexistent" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Agents.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    private static List<Agent> CreateTestAgents(int count)
    {
        var agents = new List<Agent>();
        for (int i = 0; i < count; i++)
        {
            agents.Add(Agent.Create(
                Guid.NewGuid(),
                $"Test Agent {i + 1}",
                $"Description {i + 1}",
                LLMModel.GPT4o
            ));
        }
        return agents;
    }
}
