using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;
public record UpdateUserCommand : IRequest<User> {
    public Guid UserUID { get; set; }
    public Guid RoleUID { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User> {
    private readonly IDataContext _dataContext;

    public UpdateUserCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken) {
        var user = await _dataContext.Users.SingleOrDefaultAsync(e => e.UID.Equals(request.UserUID), cancellationToken);

        if (user == null) {
            throw new NotFoundException(nameof(User), request.UserUID);
        }

        var role = await _dataContext.Roles.SingleOrDefaultAsync(e => e.UID.Equals(request.RoleUID), cancellationToken);

        if (role == null) {
            throw new NotFoundException(nameof(Role), request.RoleUID);
        }

        user.Role = role;
        user.RoleUID = role.UID;

        _dataContext.Entry(user).State = EntityState.Modified;

        await _dataContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}

internal class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand> {

    public UpdateUserCommandValidator() {
        RuleFor(e => e.RoleUID)
            .NotEmpty();
    }
}