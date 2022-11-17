using Domain.Models;
using MediatR;

namespace Application.Users.Commands.UpdateUser;
public record UpdateUserCommand : IRequest<User> {
    public Guid UserUID { get; set; }
    public Guid RoleUID { get; set; }
}