using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries.GetRoles;

public class GetRoleByAlternativeIdCommandHandler : IRequestHandler<GetRoleByAlternativeIdCommand, Role?> {
    private readonly IDataContext _dataContext;

    public GetRoleByAlternativeIdCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Role?> Handle(GetRoleByAlternativeIdCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Roles.FirstOrDefaultAsync(e => e.AlternativeId == request.AlternativeId, cancellationToken);
    }
}