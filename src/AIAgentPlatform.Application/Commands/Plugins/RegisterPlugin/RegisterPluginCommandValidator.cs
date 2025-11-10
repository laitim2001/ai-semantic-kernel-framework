using FluentValidation;

namespace AIAgentPlatform.Application.Commands.Plugins.RegisterPlugin;

/// <summary>
/// Validator for RegisterPluginCommand
/// </summary>
public sealed class RegisterPluginCommandValidator : AbstractValidator<RegisterPluginCommand>
{
    public RegisterPluginCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID cannot be empty");

        // Must provide either AssemblyPath OR (PluginId + Name + Version)
        RuleFor(x => x)
            .Must(HaveValidRegistrationSource)
            .WithMessage("Must provide either AssemblyPath or (PluginId + Name + Version)");

        // When AssemblyPath is provided
        When(x => !string.IsNullOrWhiteSpace(x.AssemblyPath), () =>
        {
            RuleFor(x => x.AssemblyPath)
                .Must(path => path != null && (path.EndsWith(".dll") || path.EndsWith(".exe")))
                .WithMessage("Assembly path must end with .dll or .exe");
        });

        // When providing manual metadata
        When(x => string.IsNullOrWhiteSpace(x.AssemblyPath), () =>
        {
            RuleFor(x => x.PluginId)
                .NotEmpty()
                .WithMessage("PluginId is required when not loading from assembly")
                .MaximumLength(100)
                .WithMessage("PluginId must not exceed 100 characters")
                .Matches("^[a-zA-Z0-9_-]+$")
                .WithMessage("PluginId can only contain letters, numbers, underscores, and hyphens");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required when not loading from assembly")
                .MaximumLength(200)
                .WithMessage("Name must not exceed 200 characters");

            RuleFor(x => x.Version)
                .NotEmpty()
                .WithMessage("Version is required when not loading from assembly")
                .Matches(@"^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?$")
                .WithMessage("Version must follow semantic versioning format (e.g., 1.0.0 or 1.0.0-beta)");
        });

        // Optional fields validation
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

    private static bool HaveValidRegistrationSource(RegisterPluginCommand command)
    {
        var hasAssemblyPath = !string.IsNullOrWhiteSpace(command.AssemblyPath);
        var hasManualMetadata = !string.IsNullOrWhiteSpace(command.PluginId) &&
                                !string.IsNullOrWhiteSpace(command.Name) &&
                                !string.IsNullOrWhiteSpace(command.Version);

        return hasAssemblyPath || hasManualMetadata;
    }
}
