using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Queries.GetChat;

public class GetChatCommandHandler : IRequestHandler<GetChatCommand, Chat?> {
    private readonly IDataContext _dataContext;

    public GetChatCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Chat?> Handle(GetChatCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Chats.FirstOrDefaultAsync(e => e.TelegramChatId.Equals(request.TelegramChatId));
    }
}