using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Commands.UpdateChat;

public class UpdateChatCommandHandler : IRequestHandler<UpdateChatCommand, Chat> {
    private readonly IDataContext _dataContext;

    public UpdateChatCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Chat> Handle(UpdateChatCommand request, CancellationToken cancellationToken) {
        var entity = await _dataContext.Chats.SingleOrDefaultAsync(e => e.UID.Equals(request.UID), cancellationToken);

        if (entity == null) {
            throw new NotFoundException(nameof(Chat), request.UID);
        }

        entity.TelegramChatId = request.TelegramChatId;
        entity.Name = request.Name;
        entity.IsJoined = request.IsJoined;
        entity.KickedByUserLogin = request.KickedUserLogin;
        entity.KickedByUserId = request.KickedUserId;
        entity.KickedTime = request.IsJoined ? null : request.Time;

        await _dataContext.SaveChangesAsync(cancellationToken);

        return entity;
    }
}