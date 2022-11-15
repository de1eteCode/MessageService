using FluentValidation;

namespace Application.Chats.Queries.GetChat;

public class GetChatCommandValidator : AbstractValidator<GetChatCommand> {

    public GetChatCommandValidator() {
        RuleFor(e => e.TelegramChatId).NotEmpty();
    }
}