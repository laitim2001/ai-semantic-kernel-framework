namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents supported LLM models
/// </summary>
public sealed class LLMModel
{
    public string Value { get; }

    // Predefined models
    public static readonly LLMModel GPT4 = new("gpt-4");
    public static readonly LLMModel GPT4o = new("gpt-4o");
    public static readonly LLMModel GPT4oMini = new("gpt-4o-mini");

    private static readonly HashSet<string> ValidModels = new()
    {
        "gpt-4",
        "gpt-4o",
        "gpt-4o-mini"
    };

    private LLMModel(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new LLMModel from string value
    /// </summary>
    public static LLMModel From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("LLM model cannot be empty", nameof(value));

        var normalizedValue = value.ToLowerInvariant();

        if (!ValidModels.Contains(normalizedValue))
            throw new ArgumentException($"Invalid LLM model: {value}. Valid models: {string.Join(", ", ValidModels)}", nameof(value));

        return new LLMModel(normalizedValue);
    }

    /// <summary>
    /// Checks if a model string is valid
    /// </summary>
    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && ValidModels.Contains(value.ToLowerInvariant());
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is LLMModel other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(LLMModel model) => model.Value;
}
