using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Commands;
public record UpdateChatCommand : IRequest<Chat> {
    public Guid UID { get; set; }

    public long TelegramChatId { get; set; }

    public string Name { get; set; } = default!;

    public bool IsJoined { get; set; } = default!;

    public string? KickedUserLogin { get; set; }

    public long? KickedUserId { get; set; } = default!;

    public DateTime Time { get; set; } = DateTime.Now;
}

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

internal class UpdateChatCommandValidator : AbstractValidator<UpdateChatCommand> {

    public UpdateChatCommandValidator() {
        RuleFor(e => e.UID)
            .NotEmpty();

        RuleFor(e => e.TelegramChatId)
            .NotEmpty();

        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(e => e.KickedUserLogin)
            .MaximumLength(255);

        RuleFor(e => e.Time)
            .NotEmpty();
    }
}