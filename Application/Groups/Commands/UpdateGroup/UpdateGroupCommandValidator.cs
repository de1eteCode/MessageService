using FluentValidation;

namespace Application.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand> {
    public UpdateGroupCommandValidator() {
        RuleFor(e => e.GroupUID)
            .NotEmpty();

        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();
    }
}