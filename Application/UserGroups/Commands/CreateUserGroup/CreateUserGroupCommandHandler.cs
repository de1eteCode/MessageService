using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UserGroups.Commands.CreateUserGroup;

public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, UserGroup> {
    private readonly IDataContext _context;

    public CreateUserGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<UserGroup> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken) {
        var userGroup = await _context.UserGroups.SingleOrDefaultAsync(e => e.GroupUID.Equals(request.GroupUID) && e.UserUID.Equals(request.UserUID), cancellationToken);

        if (userGroup != null) {
            throw new ExistingEntityException(nameof(UserGroup), userGroup.UID);
        }

        var user = await _context.Users.SingleOrDefaultAsync(e => e.UID.Equals(request.UserUID), cancellationToken);

        if (user == null) {
            throw new NotFoundException(nameof(User), request.UserUID);
        }

        var group = await _context.Groups.SingleOrDefaultAsync(e => e.UID.Equals(request.GroupUID), cancellationToken);

        if (group == null) {
            throw new NotFoundException(nameof(Group), request.GroupUID);
        }

        userGroup = new UserGroup() {
            GroupUID = request.GroupUID,
            UserUID = request.UserUID
        };

        await _context.UserGroups.AddAsync(userGroup, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return userGroup;
    }
}