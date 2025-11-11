using FluentValidation;

namespace AIAgentPlatform.Application.Commands.Plugins.UpdatePlugin;

/// <summary>
/// Validator for UpdatePluginCommand
/// </summary>
public sealed class UpdatePluginCommandValidator : AbstractValidator<UpdatePluginCommand>
{
    public UpdatePluginCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Plugin ID cannot be empty");

        // At least one field must be provided for update
        RuleFor(x => x)
            .Must(HaveAtLeastOneUpdateField)
            .WithMessage("At least one field (Name, Description, or Category) must be provided for update");

        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(200)
                .WithMessage("Name must not exceed 200 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Category), () =>
        {
            RuleFor(x => x.Category)
                .MaximumLength(100)
                .WithMessage("Category must not exceed 100 characters");
        });
    }

    private static bool HaveAtLeastOneUpdateField(UpdatePluginCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.Name) ||
               !string.IsNullOrWhiteSpace(command.Description) ||
               !string.IsNullOrWhiteSpace(command.Category);
    }
}
