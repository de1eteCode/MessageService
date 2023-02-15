using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Queries;
public record GetChatGroupCommand : IRequest<ChatGroup?> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
}

internal class GetChatGroupCommandHandler : IRequestHandler<GetChatGroupCommand, ChatGroup?> {
    private readonly IDataContext _context;

    public GetChatGroupCommandHandler(IDataContext context) {
        _context = context;
    }

    public Task<ChatGroup?> Handle(GetChatGroupCommand request, CancellationToken cancellationToken) {
        return _context.ChatGroups.SingleOrDefaultAsync(e => e.GroupUID.Equals(request.GroupUID) && e.ChatUID.Equals(request.ChatUID));
    }
}

internal class GetChatGroupCommandValidator : AbstractValidator<GetChatGroupCommand> {

    public GetChatGroupCommandValidator() {
        RuleFor(e => e.ChatUID)
            .NotEmpty();

        RuleFor(e => e.GroupUID)
            .NotEmpty();
    }
}