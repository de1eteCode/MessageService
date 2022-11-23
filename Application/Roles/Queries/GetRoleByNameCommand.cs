using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;

public record GetRoleByNameCommand : IRequest<Role?> {
    public string Name { get; set; } = default!;
}

public class GetRoleByNameCommandHandler : IRequestHandler<GetRoleByNameCommand, Role?> {
    private readonly IDataContext _dataContext;

    public GetRoleByNameCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Role?> Handle(GetRoleByNameCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Roles.FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);
    }
}

public class GetRoleByNameCommandValidator : AbstractValidator<GetRoleByNameCommand> {

    public GetRoleByNameCommandValidator() {
        RuleFor(x => x.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();
    }
}