﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, Group> {
    private readonly IDataContext _context;

    public UpdateGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<Group> Handle(UpdateGroupCommand request, CancellationToken cancellationToken) {
        var group = await _context.Groups.SingleOrDefaultAsync(e => e.UID.Equals(request.GroupUID), cancellationToken);

        if (group == null) {
            throw new NotFoundException(nameof(Group), request.GroupUID);
        }

        group.Name = request.Name;

        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);

        return group;

        throw new NotImplementedException();
    }
}