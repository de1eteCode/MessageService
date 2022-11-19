using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Queries.GetChat;

public class GetChatsCommandHandler : IRequestHandler<GetChatsCommand, IEnumerable<Chat>> {
    private readonly IDataContext _dataContext;

    public GetChatsCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Chat>> Handle(GetChatsCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Chats.ToListAsync();
    }
}