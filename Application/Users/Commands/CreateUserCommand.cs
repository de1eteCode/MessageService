using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<User> {
    public string Name { get; set; } = default!;
    public long TelegramId { get; set; }
    public Guid RoleUID { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User> {
    private readonly IDataContext _dataContext;

    public CreateUserCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
        var role = await _dataContext.Roles.SingleOrDefaultAsync(e => e.UID.Equals(request.RoleUID), cancellationToken);

        if (role == null) {
            throw new NotFoundException(nameof(Role), request.RoleUID);
        }

        var user = new User() {
            Name = request.Name,
            TelegramId = request.TelegramId,
            Role = role,
            RoleUID = role.UID
        };

        _dataContext.Users.Add(user);

        await _dataContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand> {

    public CreateUserCommandValidator() {
        RuleFor(e => e.TelegramId)
            .NotEmpty();

        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(e => e.RoleUID)
            .NotEmpty();
    }
}