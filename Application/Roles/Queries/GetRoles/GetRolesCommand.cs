using Domain.Models;
using MediatR;

namespace Application.Roles.Queries.GetRoles;
public record GetRolesCommand : IRequest<IEnumerable<Role>> {
}