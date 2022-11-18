using Domain.Models;
using MediatR;

namespace Application.Groups.Queries.GetGroup;

public record GetGroupsCommand : IRequest<IEnumerable<Group>> {
}