using FluentValidation;

namespace Application.Users.Queries.GetUser;

public class GetUserCommandValidator : AbstractValidator<GetUserCommand> {

    public GetUserCommandValidator() {
        RuleFor(e => e.TelegramId)
            .NotEmpty();
    }
}