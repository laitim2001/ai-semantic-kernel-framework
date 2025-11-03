using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents an AI Agent with configuration and behavior settings
/// </summary>
public sealed class Agent : BaseEntity
{
    /// <summary>
    /// Owner of the agent
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Agent display name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Agent description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// System instructions/prompt for the agent
    /// </summary>
    public string Instructions { get; private set; }

    /// <summary>
    /// LLM model to use
    /// </summary>
    public LLMModel Model { get; private set; }

    /// <summary>
    /// Temperature parameter (0-2, controls randomness)
    /// </summary>
    public decimal Temperature { get; private set; }

    /// <summary>
    /// Maximum tokens for response
    /// </summary>
    public int MaxTokens { get; private set; }

    /// <summary>
    /// Current status of the agent
    /// </summary>
    public AgentStatus Status { get; private set; }

    // Private constructor for EF Core
    private Agent()
    {
        Name = string.Empty;
        Instructions = string.Empty;
        Model = LLMModel.GPT4o;
        Status = AgentStatus.Active;
    }

    /// <summary>
    /// Creates a new Agent instance
    /// </summary>
    public static Agent Create(
        Guid userId,
        string name,
        string instructions,
        LLMModel model,
        decimal temperature = 0.7m,
        int maxTokens = 4096,
        string? description = null)
    {
        ValidateName(name);
        ValidateInstructions(instructions);
        ValidateTemperature(temperature);
        ValidateMaxTokens(maxTokens);

        return new Agent
        {
            UserId = userId,
            Name = name.Trim(),
            Description = description?.Trim(),
            Instructions = instructions.Trim(),
            Model = model,
            Temperature = temperature,
            MaxTokens = maxTokens,
            Status = AgentStatus.Active
        };
    }

    /// <summary>
    /// Updates agent basic information
    /// </summary>
    public void Update(string name, string? description, string instructions)
    {
        ValidateName(name);
        ValidateInstructions(instructions);

        Name = name.Trim();
        Description = description?.Trim();
        Instructions = instructions.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Updates agent model configuration
    /// </summary>
    public void UpdateModelConfiguration(LLMModel model, decimal temperature, int maxTokens)
    {
        ValidateTemperature(temperature);
        ValidateMaxTokens(maxTokens);

        Model = model;
        Temperature = temperature;
        MaxTokens = maxTokens;
        MarkAsModified();
    }

    /// <summary>
    /// Activates the agent
    /// </summary>
    public void Activate()
    {
        if (Status.IsActive)
            return;

        Status = AgentStatus.Active;
        MarkAsModified();
    }

    /// <summary>
    /// Pauses the agent
    /// </summary>
    public void Pause()
    {
        if (Status.IsPaused)
            return;

        Status = AgentStatus.Paused;
        MarkAsModified();
    }

    /// <summary>
    /// Archives the agent (soft delete)
    /// </summary>
    public void Archive()
    {
        if (Status.IsArchived)
            return;

        Status = AgentStatus.Archived;
        MarkAsModified();
    }

    // Validation methods
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Agent name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Agent name cannot exceed 100 characters", nameof(name));
    }

    private static void ValidateInstructions(string instructions)
    {
        if (string.IsNullOrWhiteSpace(instructions))
            throw new ArgumentException("Agent instructions cannot be empty", nameof(instructions));

        if (instructions.Length > 10000)
            throw new ArgumentException("Agent instructions cannot exceed 10000 characters", nameof(instructions));
    }

    private static void ValidateTemperature(decimal temperature)
    {
        if (temperature < 0 || temperature > 2)
            throw new ArgumentException("Temperature must be between 0 and 2", nameof(temperature));
    }

    private static void ValidateMaxTokens(int maxTokens)
    {
        if (maxTokens <= 0)
            throw new ArgumentException("MaxTokens must be greater than 0", nameof(maxTokens));

        if (maxTokens > 128000) // GPT-4o max context
            throw new ArgumentException("MaxTokens cannot exceed 128000", nameof(maxTokens));
    }
}
