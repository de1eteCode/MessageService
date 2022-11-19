using Domain.Models;
using MediatR;

namespace Application.Groups.Commands.UpdateGroup;
public record UpdateGroupCommand : IRequest<Group> {
    public Guid GroupUID { get; set; }
    public string Name { get; set; } = default!;
}