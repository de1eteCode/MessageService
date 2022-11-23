using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Queries;
public record GetRolesCommand : IRequest<IEnumerable<Role>> {
    public Func<Role, bool>? Predicate { get; set; } = null;
}

public class GetRolesCommandHandler : IRequestHandler<GetRolesCommand, IEnumerable<Role>> {
    private readonly IDataContext _dataContext;

    public GetRolesCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Role>> Handle(GetRolesCommand request, CancellationToken cancellationToken) {
        if (request.Predicate != null) {
            return await _dataContext.Roles.Where(request.Predicate).AsQueryable().ToListAsync();
        }
        else {
            return await _dataContext.Roles.ToListAsync(cancellationToken);
        }
    }
}