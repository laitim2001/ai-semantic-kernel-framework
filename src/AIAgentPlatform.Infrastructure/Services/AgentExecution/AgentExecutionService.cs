using AIAgentPlatform.Application.AgentExecutions.Services;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using AIAgentPlatform.Infrastructure.Services.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace AIAgentPlatform.Infrastructure.Services.AgentExecution;

/// <summary>
/// Implementation of agent execution service
/// </summary>
public sealed class AgentExecutionService : IAgentExecutionService
{
    private readonly ISemanticKernelService _semanticKernelService;
    private readonly IAgentExecutionRepository _executionRepository;
    private readonly ILogger<AgentExecutionService> _logger;

    public AgentExecutionService(
        ISemanticKernelService semanticKernelService,
        IAgentExecutionRepository executionRepository,
        ILogger<AgentExecutionService> logger)
    {
        _semanticKernelService = semanticKernelService;
        _executionRepository = executionRepository;
        _logger = logger;
    }

    public async Task<(Domain.Entities.AgentExecution Execution, string Output)> ExecuteAsync(
        Agent agent,
        Guid conversationId,
        string input,
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting agent execution for Agent {AgentId} in Conversation {ConversationId}",
            agent.Id, conversationId);

        // Create execution record
        var execution = Domain.Entities.AgentExecution.Create(
            agentId: agent.Id,
            conversationId: conversationId,
            metadata: metadata
        );

        try
        {
            // Save initial execution record
            await _executionRepository.AddAsync(execution, cancellationToken);
            _logger.LogDebug("Created execution record {ExecutionId}", execution.Id);

            // Create Semantic Kernel
            var kernel = await _semanticKernelService.CreateKernelAsync(agent, cancellationToken);

            // Build prompt with agent instructions
            var fullPrompt = BuildPrompt(agent, input);

            // Execute with Semantic Kernel
            var (output, tokensUsed) = await _semanticKernelService.ExecutePromptAsync(
                kernel,
                fullPrompt,
                cancellationToken);

            // Mark as completed
            execution.MarkAsCompleted(tokensUsed);
            await _executionRepository.UpdateAsync(execution, cancellationToken);

            _logger.LogInformation("Agent execution {ExecutionId} completed successfully. Tokens: {TokensUsed}, Time: {ResponseTime}ms",
                execution.Id, tokensUsed, execution.ResponseTimeMs);

            return (execution, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent execution {ExecutionId} failed: {ErrorMessage}",
                execution.Id, ex.Message);

            // Mark as failed
            var errorMessage = ex.Message.Length > 2000
                ? ex.Message.Substring(0, 2000)
                : ex.Message;

            execution.MarkAsFailed(errorMessage);
            await _executionRepository.UpdateAsync(execution, cancellationToken);

            throw;
        }
    }

    private static string BuildPrompt(Agent agent, string userInput)
    {
        return $"""
        {agent.Instructions}

        User Input: {userInput}
        """;
    }
}
