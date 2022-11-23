using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Queries;

public record GetChatsCommand : IRequest<IEnumerable<Chat>> {
    public Func<Chat, bool>? Predicate { get; set; } = null;
}

public class GetChatsCommandHandler : IRequestHandler<GetChatsCommand, IEnumerable<Chat>> {
    private readonly IDataContext _dataContext;

    public GetChatsCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Chat>> Handle(GetChatsCommand request, CancellationToken cancellationToken) {
        if (request.Predicate != null) {
            return await _dataContext.Chats.Where(request.Predicate).AsQueryable().ToListAsync();
        }
        else {
            return await _dataContext.Chats.ToListAsync(cancellationToken);
        }
    }
}