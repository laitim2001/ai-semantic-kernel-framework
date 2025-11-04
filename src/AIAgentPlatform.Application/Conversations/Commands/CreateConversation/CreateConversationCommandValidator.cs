using FluentValidation;

namespace AIAgentPlatform.Application.Conversations.Commands.CreateConversation;

/// <summary>
/// 創建對話 Command 驗證器
/// </summary>
public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty()
            .WithMessage("Agent ID 不能為空");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID 不能為空");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("對話標題不能為空")
            .MaximumLength(200)
            .WithMessage("對話標題不能超過 200 個字元");
    }
}
