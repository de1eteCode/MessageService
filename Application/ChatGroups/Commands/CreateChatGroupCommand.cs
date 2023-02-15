using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Commands;
public record CreateChatGroupCommand : IRequest<ChatGroup> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
    public bool IsDeleted { get; set; }
}

public class CreateChatGroupCommandHandler : IRequestHandler<CreateChatGroupCommand, ChatGroup> {
    private readonly IDataContext _context;

    public CreateChatGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<ChatGroup> Handle(CreateChatGroupCommand request, CancellationToken cancellationToken) {
        var chatGroup = await _context.ChatGroups.SingleOrDefaultAsync(e => e.GroupUID.Equals(request.GroupUID) && e.ChatUID.Equals(request.ChatUID), cancellationToken);

        if (chatGroup != null) {
            throw new ExistingEntityException(nameof(ChatGroup), chatGroup.UID);
        }

        var chat = await _context.Chats.SingleOrDefaultAsync(e => e.UID.Equals(request.ChatUID), cancellationToken);

        if (chat == null) {
            throw new NotFoundException(nameof(Chat), request.ChatUID);
        }

        var group = await _context.Groups.SingleOrDefaultAsync(e => e.UID.Equals(request.GroupUID), cancellationToken);

        if (group == null) {
            throw new NotFoundException(nameof(Group), request.GroupUID);
        }

        chatGroup = new ChatGroup() {
            ChatUID = chat.UID,
            Chat = chat,
            Group = group,
            GroupUID = group.UID
        };

        await _context.ChatGroups.AddAsync(chatGroup, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return chatGroup;
    }
}

internal class CreateChatGroupCommandValidator : AbstractValidator<CreateChatGroupCommand> {

    public CreateChatGroupCommandValidator() {
        RuleFor(x => x.ChatUID)
            .NotEmpty();

        RuleFor(x => x.GroupUID)
            .NotEmpty();
    }
}