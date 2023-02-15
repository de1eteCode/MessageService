using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

public record GetUserCommand : IRequest<User?> {
    public long TelegramId { get; set; }
}

public class GetUserCommandHandler : IRequestHandler<GetUserCommand, User?> {
    private readonly IDataContext _dataContext;

    public GetUserCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<User?> Handle(GetUserCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Users.FirstOrDefaultAsync(e => e.TelegramId.Equals(request.TelegramId));
    }
}

internal class GetUserCommandValidator : AbstractValidator<GetUserCommand> {

    public GetUserCommandValidator() {
        RuleFor(e => e.TelegramId)
            .NotEmpty();
    }
}