using FluentValidation;

namespace Application.ChatGroups.Queries.GetChatGroup;

public class GetChatGroupCommandValidator : AbstractValidator<GetChatGroupCommand> {

    public GetChatGroupCommandValidator() {
        RuleFor(e => e.ChatUID)
            .NotEmpty();

        RuleFor(e => e.GroupUID)
            .NotEmpty();
    }
}