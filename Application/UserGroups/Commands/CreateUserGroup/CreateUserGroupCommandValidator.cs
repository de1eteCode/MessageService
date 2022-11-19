using FluentValidation;

namespace Application.UserGroups.Commands.CreateUserGroup;

public class CreateUserGroupCommandValidator : AbstractValidator<CreateUserGroupCommand> {
    public CreateUserGroupCommandValidator() {
        RuleFor(e => e.UserUID)
            .NotEmpty();

        RuleFor(e => e.GroupUID)
            .NotEmpty();
    }
}