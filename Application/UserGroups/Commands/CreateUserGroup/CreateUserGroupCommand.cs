using Domain.Models;
using MediatR;

namespace Application.UserGroups.Commands.CreateUserGroup;
public record CreateUserGroupCommand : IRequest<UserGroup> {
    public Guid UserUID { get; set; }
    public Guid GroupUID { get; set; }
}