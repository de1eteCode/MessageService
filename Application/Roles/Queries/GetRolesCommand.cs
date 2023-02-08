using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;
public record GetRolesCommand : IRequest<IEnumerable<Role>>;

public class GetRolesCommandHandler : IRequestHandler<GetRolesCommand, IEnumerable<Role>> {
    private readonly IDataContext _dataContext;

    public GetRolesCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Role>> Handle(GetRolesCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Roles.ToListAsync(cancellationToken);
    }
}