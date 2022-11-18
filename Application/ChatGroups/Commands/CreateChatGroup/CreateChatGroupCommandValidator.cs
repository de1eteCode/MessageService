using FluentValidation;

namespace Application.ChatGroups.Commands.CreateChatGroup;

public class CreateChatGroupCommandValidator : AbstractValidator<CreateChatGroupCommand> {

    public CreateChatGroupCommandValidator() {
        RuleFor(x => x.ChatUID)
            .NotEmpty();

        RuleFor(x => x.GroupUID)
            .NotEmpty();
    }
}