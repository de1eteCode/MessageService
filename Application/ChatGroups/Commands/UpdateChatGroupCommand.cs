using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Commands;
public record UpdateChatGroupCommand : IRequest<ChatGroup> {
    public Guid ChatGroupUID { get; set; }
    public bool IsDeleted { get; set; }
}

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