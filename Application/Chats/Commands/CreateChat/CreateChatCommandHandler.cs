using Application.Common.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Chats.Commands.CreateChat;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Chat> {
    private readonly IDataContext _dataContext;

    public CreateChatCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Chat> Handle(CreateChatCommand request, CancellationToken cancellationToken) {
        var entity = new Chat() {
            Name = request.Name,
            IsJoined = request.IsJoined,
            KickedByUserId = request.KickedUserId,
            KickedByUserLogin = request.KickedUserLogin,
            TelegramChatId = request.TelegramChatId,
            KickedTime = request.Time
        };

        // event ?

        _dataContext.Chats.Add(entity);

        await _dataContext.SaveChangesAsync(cancellationToken);

        return entity;
    }
}