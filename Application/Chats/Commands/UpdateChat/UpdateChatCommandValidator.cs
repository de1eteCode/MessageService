using FluentValidation;

namespace Application.Chats.Commands.UpdateChat;

public class UpdateChatCommandValidator : AbstractValidator<UpdateChatCommand> {

    public UpdateChatCommandValidator() {
        RuleFor(e => e.UID)
            .NotEmpty();

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