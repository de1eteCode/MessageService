using Application.Common.Interfaces;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ChatGroups.Queries;
public record GetChatGroupCommand : IRequest<ChatGroup?> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
}

public class GetChatGroupCommandHandler : IRequestHandler<GetChatGroupCommand, ChatGroup?> {
    private readonly IDataContext _dataContext;

    public GetChatGroupCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<ChatGroup?> Handle(GetChatGroupCommand request, CancellationToken cancellationToken) {
        return await _dataContext.ChatGroups.SingleOrDefaultAsync(e => e.ChatUID.Equals(request.ChatUID) && e.GroupUID.Equals(request.GroupUID));
    }
}

public class GetChatGroupCommandValidator : AbstractValidator<GetChatGroupCommand> {

    public GetChatGroupCommandValidator() {
        RuleFor(e => e.ChatUID)
            .NotEmpty();

        RuleFor(e => e.GroupUID)
            .NotEmpty();
    }
}