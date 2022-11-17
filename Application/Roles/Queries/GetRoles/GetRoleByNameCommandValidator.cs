using FluentValidation;

namespace Application.Roles.Queries.GetRoles;

public class GetRoleByNameCommandValidator : AbstractValidator<GetRoleByNameCommand> {

    public GetRoleByNameCommandValidator() {
        RuleFor(x => x.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();
    }
}