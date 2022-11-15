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
            .MinimumLength(1)
            .MaximumLength(255)
            .Empty();

        RuleFor(e => e.KickedUserId)
            .Empty();

        RuleFor(e => e.IsJoined)
            .NotEmpty();

        RuleFor(e => e.Time)
            .NotEmpty();
    }
}