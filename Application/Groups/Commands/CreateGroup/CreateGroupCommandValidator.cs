using FluentValidation;

namespace Application.Groups.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand> {

    public CreateGroupCommandValidator() {
        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(e => e.OwnerUserUID)
            .NotEmpty();
    }
}