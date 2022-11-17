using Domain.Models;
using MediatR;

namespace Application.Roles.Queries.GetRoles;

public record GetRoleByNameCommand : IRequest<Role?> {
    public string Name { get; set; } = default!;
}