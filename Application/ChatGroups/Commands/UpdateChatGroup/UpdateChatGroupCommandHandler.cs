using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Commands.UpdateChatGroup;

public class UpdateChatGroupCommandHandler : IRequestHandler<UpdateChatGroupCommand, ChatGroup> {
    private readonly IDataContext _context;

    public UpdateChatGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<ChatGroup> Handle(UpdateChatGroupCommand request, CancellationToken cancellationToken) {
        var chatGroup = await _context.ChatGroups.SingleOrDefaultAsync(e => e.UID.Equals(request.ChatGroupUID), cancellationToken);

        if (chatGroup == null) {
            throw new NotFoundException(nameof(ChatGroup), request.ChatGroupUID);
        }

        chatGroup.IsDeleted = request.IsDeleted;

        _context.Entry(chatGroup).State = EntityState.Modified;

        await _context.SaveChangesAsync(cancellationToken);

        return chatGroup;
    }
}