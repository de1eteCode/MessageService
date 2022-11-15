using FluentValidation;

namespace Application.Chats.Commands.CreateChat;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand> {

    public CreateChatCommandValidator() {
        RuleFor(e => e.TelegramChatId)
            .NotEmpty();

        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(e => e.KickedUserLogin)
            .MaximumLength(255);

        RuleFor(e => e.Time)
            .NotEmpty();
    }
}