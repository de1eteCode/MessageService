using Domain.Models;
using MediatR;

namespace Application.Roles.Queries.GetRoles;

public record GetRoleByAlternativeIdCommand : IRequest<Role?> {
    public int AlternativeId { get; set; }
}