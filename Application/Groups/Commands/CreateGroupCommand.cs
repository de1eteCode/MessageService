﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands.CreateGroup;
public record CreateGroupCommand : IRequest<Group> {
    public string Name { get; set; } = default!;
    public Guid OwnerUserUID { get; set; }
}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Group> {
    private readonly IDataContext _dataContext;

    public CreateGroupCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<Group> Handle(CreateGroupCommand request, CancellationToken cancellationToken) {
        var owner = await _dataContext.Users.SingleOrDefaultAsync(e => e.UID.Equals(request.OwnerUserUID), cancellationToken);

        if (owner == null) {
            throw new NotFoundException(nameof(User), request.OwnerUserUID);
        }

        var group = new Group() {
            Name = request.Name
        };

        var userGroup = new UserGroup() {
            Group = group,
            User = owner
        };

        await _dataContext.Groups.AddAsync(group, cancellationToken);
        await _dataContext.UserGroups.AddAsync(userGroup, cancellationToken);

        await _dataContext.SaveChangesAsync(cancellationToken);

        return group;
    }
}

internal class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand> {

    public CreateGroupCommandValidator() {
        RuleFor(e => e.Name)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty();

        RuleFor(e => e.OwnerUserUID)
            .NotEmpty();
    }
}