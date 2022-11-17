using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand> {

    public UpdateUserCommandValidator() {
        RuleFor(e => e.RoleUID)
            .NotEmpty();
    }
}