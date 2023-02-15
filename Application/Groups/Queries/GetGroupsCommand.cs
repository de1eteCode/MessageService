using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries;

public record GetGroupsCommand : IRequest<IEnumerable<Group>>;

public class GetGroupsCommandHandler : IRequestHandler<GetGroupsCommand, IEnumerable<Group>> {
    private readonly IDataContext _context;

    public GetGroupsCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<IEnumerable<Group>> Handle(GetGroupsCommand request, CancellationToken cancellationToken) {
        return await _context.Groups.ToListAsync(cancellationToken);
    }
}