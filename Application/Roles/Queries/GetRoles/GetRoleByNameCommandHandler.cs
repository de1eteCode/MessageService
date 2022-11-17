using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries.GetRoles;

public class GetRoleByNameCommandHandler : IRequestHandler<GetRoleByNameCommand, Role?> {
    private readonly IDataContext _dataContext;

    public GetRoleByNameCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Role?> Handle(GetRoleByNameCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Roles.FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);
    }
}