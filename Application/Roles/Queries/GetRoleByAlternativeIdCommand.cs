using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;

public record GetRoleByAlternativeIdCommand : IRequest<Role?> {
    public int AlternativeId { get; set; }
}

public class GetRoleByAlternativeIdCommandHandler : IRequestHandler<GetRoleByAlternativeIdCommand, Role?> {
    private readonly IDataContext _dataContext;

    public GetRoleByAlternativeIdCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Role?> Handle(GetRoleByAlternativeIdCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Roles.FirstOrDefaultAsync(e => e.AlternativeId == request.AlternativeId, cancellationToken);
    }
}

public class GetRoleByAlternativeIdCommandValidator : AbstractValidator<GetRoleByAlternativeIdCommand> {

    public GetRoleByAlternativeIdCommandValidator() {
        RuleFor(x => x.AlternativeId)
            .NotEmpty();
    }
}