using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AIAgentPlatform.UnitTests.Application.Commands;

public sealed class CreateAgentCommandHandlerTests
{
    private readonly Mock<IAgentRepository> _repositoryMock;
    private readonly CreateAgentCommandHandler _handler;

    public CreateAgentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAgentRepository>();
        _handler = new CreateAgentCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateAgent()
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Description = "Test Description",
            Instructions = "You are a helpful assistant",
            Model = "gpt-4o",
            Temperature = 0.7m,
            MaxTokens = 4096
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agent agent, CancellationToken _) => agent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(command.UserId);
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.Instructions.Should().Be(command.Instructions);
        result.Model.Should().Be(command.Model);
        result.Temperature.Should().Be(command.Temperature);
        result.MaxTokens.Should().Be(command.MaxTokens);
        result.Status.Should().Be("active");

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithInvalidModel_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateAgentCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Agent",
            Instructions = "You are a helpful assistant",
            Model = "invalid-model"
        };

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<ArgumentException>();

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Agent>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
