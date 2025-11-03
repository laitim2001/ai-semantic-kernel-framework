using AIAgentPlatform.Domain.ValueObjects;
using FluentValidation;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Validator for CreateAgentCommand
/// </summary>
public sealed class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Agent name is required")
            .MaximumLength(100)
            .WithMessage("Agent name cannot exceed 100 characters");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("Instructions are required")
            .MaximumLength(10000)
            .WithMessage("Instructions cannot exceed 10000 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .Must(model => LLMModel.IsValid(model))
            .WithMessage("Invalid LLM model. Valid models: gpt-4, gpt-4o, gpt-4o-mini");

        RuleFor(x => x.Temperature)
            .InclusiveBetween(0m, 2m)
            .WithMessage("Temperature must be between 0 and 2");

        RuleFor(x => x.MaxTokens)
            .GreaterThan(0)
            .WithMessage("MaxTokens must be greater than 0")
            .LessThanOrEqualTo(128000)
            .WithMessage("MaxTokens cannot exceed 128000");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 500 characters");
    }
}
