using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries;

public record GetGroupsCommand : IRequest<IEnumerable<Group>> {
    public Func<Group, bool>? Predicate { get; set; } = null;
}

public class GetGroupsCommandHandler : IRequestHandler<GetGroupsCommand, IEnumerable<Group>> {
    private readonly IDataContext _context;

    public GetGroupsCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<IEnumerable<Group>> Handle(GetGroupsCommand request, CancellationToken cancellationToken) {
        if (request.Predicate != null) {
            return await _context.Groups.Where(request.Predicate).AsQueryable().ToListAsync();
        }
        else {
            return await _context.Groups.ToListAsync(cancellationToken);
        }
    }
}