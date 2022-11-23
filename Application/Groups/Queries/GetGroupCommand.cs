using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries;
public record GetGroupCommand : IRequest<Group?> {
    public int AlternativeId { get; set; }
}

public class GetGroupCommandHandler : IRequestHandler<GetGroupCommand, Group?> {
    private readonly IDataContext _context;

    public GetGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<Group?> Handle(GetGroupCommand request, CancellationToken cancellationToken) {
        return await _context.Groups.SingleOrDefaultAsync(e => e.AlternativeId.Equals(request.AlternativeId), cancellationToken);
    }
}