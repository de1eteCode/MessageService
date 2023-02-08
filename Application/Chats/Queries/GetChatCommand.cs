using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chats.Queries;
public record GetChatCommand : IRequest<Chat?> {
    public long TelegramChatId { get; set; }
}

public class GetChatCommandHandler : IRequestHandler<GetChatCommand, Chat?> {
    private readonly IDataContext _dataContext;

    public GetChatCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Chat?> Handle(GetChatCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Chats.FirstOrDefaultAsync(e => e.TelegramChatId.Equals(request.TelegramChatId));
    }
}

internal class GetChatCommandValidator : AbstractValidator<GetChatCommand> {

    public GetChatCommandValidator() {
        RuleFor(e => e.TelegramChatId).NotEmpty();
    }
}