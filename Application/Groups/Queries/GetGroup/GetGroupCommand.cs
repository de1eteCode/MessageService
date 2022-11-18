using Domain.Models;
using MediatR;

namespace Application.Groups.Queries.GetGroup;
public record GetGroupCommand : IRequest<Group?> {
    public int AlternativeId { get; set; }
}