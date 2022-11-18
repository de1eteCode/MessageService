using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Queries.GetChatGroup;

public class GetChatGroupCommandHandler : IRequestHandler<GetChatGroupCommand, ChatGroup?> {
    private readonly IDataContext _context;

    public GetChatGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public Task<ChatGroup?> Handle(GetChatGroupCommand request, CancellationToken cancellationToken) {
        return _context.ChatGroups.SingleOrDefaultAsync(e => e.GroupUID.Equals(request.GroupUID) && e.ChatUID.Equals(request.ChatUID));
    }
}