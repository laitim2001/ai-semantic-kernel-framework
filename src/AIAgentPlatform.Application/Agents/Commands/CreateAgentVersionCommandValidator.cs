using FluentValidation;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// Validator for CreateAgentVersionCommand
/// </summary>
public sealed class CreateAgentVersionCommandValidator : AbstractValidator<CreateAgentVersionCommand>
{
    private static readonly string[] ValidChangeTypes = { "major", "minor", "patch", "rollback", "hotfix" };

    public CreateAgentVersionCommandValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty()
            .WithMessage("Agent ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ChangeDescription)
            .NotEmpty()
            .WithMessage("Change description is required")
            .MaximumLength(500)
            .WithMessage("Change description cannot exceed 500 characters");

        RuleFor(x => x.ChangeType)
            .NotEmpty()
            .WithMessage("Change type is required")
            .Must(BeValidChangeType)
            .WithMessage($"Invalid change type. Must be one of: {string.Join(", ", ValidChangeTypes)}");
    }

    private bool BeValidChangeType(string changeType)
    {
        if (string.IsNullOrWhiteSpace(changeType))
            return false;

        return ValidChangeTypes.Contains(changeType.ToLowerInvariant());
    }
}
