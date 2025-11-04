using FluentValidation;

namespace AIAgentPlatform.Application.Conversations.Commands.AddMessage;

/// <summary>
/// 新增訊息 Command 驗證器
/// </summary>
public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID 不能為空");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("訊息角色不能為空")
            .Must(role => role == "user" || role == "assistant" || role == "system")
            .WithMessage("訊息角色必須是 user, assistant 或 system");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("訊息內容不能為空")
            .MaximumLength(32000)
            .WithMessage("訊息內容不能超過 32000 個字元");
    }
}
