using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

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