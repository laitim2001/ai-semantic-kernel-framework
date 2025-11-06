using AIAgentPlatform.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIAgentPlatform.Infrastructure.Services.SemanticKernel;

/// <summary>
/// Implementation of Semantic Kernel service
/// </summary>
public sealed class SemanticKernelService : ISemanticKernelService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SemanticKernelService> _logger;

    public SemanticKernelService(
        IConfiguration configuration,
        ILogger<SemanticKernelService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Kernel> CreateKernelAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Semantic Kernel for Agent {AgentId} with model {Model}",
            agent.Id, agent.Model.Value);

        var apiKey = _configuration["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Azure OpenAI API Key not configured");

        var endpoint = _configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Azure OpenAI Endpoint not configured");

        var builder = Kernel.CreateBuilder();

        // Add Azure OpenAI Chat Completion
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: agent.Model.Value,
            endpoint: endpoint,
            apiKey: apiKey
        );

        var kernel = builder.Build();

        _logger.LogDebug("Kernel created successfully for Agent {AgentId}", agent.Id);

        return await Task.FromResult(kernel);
    }

    public async Task<(string Output, int TokensUsed)> ExecutePromptAsync(
        Kernel kernel,
        string prompt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing prompt with Semantic Kernel");
        _logger.LogDebug("Prompt: {Prompt}", prompt);

        try
        {
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Create chat history
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(prompt);

            // Get response
            var response = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                cancellationToken: cancellationToken);

            // Extract output
            var output = response.Content ?? string.Empty;

            // Get token usage (if available)
            var tokensUsed = 0;
            if (response.Metadata?.ContainsKey("Usage") == true)
            {
                var usage = response.Metadata["Usage"];
                if (usage != null)
                {
                    var usageType = usage.GetType();
                    var totalTokensProperty = usageType.GetProperty("TotalTokens");
                    if (totalTokensProperty != null)
                    {
                        tokensUsed = (int)(totalTokensProperty.GetValue(usage) ?? 0);
                    }
                }
            }

            if (tokensUsed == 0)
            {
                // Rough estimate if metadata not available: ~4 chars per token
                tokensUsed = (prompt.Length + output.Length) / 4;
                _logger.LogWarning("Token usage metadata not available, using estimate: {TokensUsed}", tokensUsed);
            }

            _logger.LogInformation("Prompt execution completed. Tokens used: {TokensUsed}", tokensUsed);

            return (output, tokensUsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing prompt with Semantic Kernel");
            throw;
        }
    }
}
