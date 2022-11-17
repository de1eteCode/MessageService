using FluentValidation;

namespace Application.Roles.Queries.GetRoles;

public class GetRoleByAlternativeIdCommandValidator : AbstractValidator<GetRoleByAlternativeIdCommand> {

    public GetRoleByAlternativeIdCommandValidator() {
        RuleFor(x => x.AlternativeId)
            .NotEmpty();
    }
}