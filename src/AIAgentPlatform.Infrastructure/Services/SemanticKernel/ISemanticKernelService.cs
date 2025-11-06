using AIAgentPlatform.Domain.Entities;
using Microsoft.SemanticKernel;

namespace AIAgentPlatform.Infrastructure.Services.SemanticKernel;

/// <summary>
/// Service interface for Semantic Kernel operations
/// </summary>
public interface ISemanticKernelService
{
    /// <summary>
    /// Creates a Kernel instance configured for the given agent
    /// </summary>
    /// <param name="agent">The agent configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configured Kernel instance</returns>
    Task<Kernel> CreateKernelAsync(Agent agent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a prompt using the given kernel
    /// </summary>
    /// <param name="kernel">The kernel instance</param>
    /// <param name="prompt">The prompt to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with output text and token usage</returns>
    Task<(string Output, int TokensUsed)> ExecutePromptAsync(
        Kernel kernel,
        string prompt,
        CancellationToken cancellationToken = default);
}
