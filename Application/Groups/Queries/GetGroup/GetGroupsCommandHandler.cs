﻿using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Groups.Queries.GetGroup;

public class GetGroupsCommandHandler : IRequestHandler<GetGroupsCommand, IEnumerable<Group>> {
    private readonly IDataContext _context;

    public GetGroupsCommandHandler(IDataContext context) {
        _context = context;
    }

    public async Task<IEnumerable<Group>> Handle(GetGroupsCommand request, CancellationToken cancellationToken) {
        return await _context.Groups.ToListAsync(cancellationToken);
    }
}