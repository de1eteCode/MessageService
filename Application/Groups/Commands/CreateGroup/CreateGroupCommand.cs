using Domain.Models;
using MediatR;

namespace Application.Groups.Commands.CreateGroup;
public record CreateGroupCommand : IRequest<Group> {
    public string Name { get; set; } = default!;
    public Guid OwnerUserUID { get; set; }
}