using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Chats.Commands;
public record CreateChatCommand : IRequest<Chat> {
    public long TelegramChatId { get; set; }

    public string Name { get; set; } = default!;

    public bool IsJoined { get; set; } = default!;

    public string? KickedUserLogin { get; set; }

    public long? KickedUserId { get; set; } = default!;

    public DateTime Time { get; set; } = DateTime.Now;
}

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

internal class CreateChatCommandValidator : AbstractValidator<CreateChatCommand> {

    public CreateChatCommandValidator() {
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