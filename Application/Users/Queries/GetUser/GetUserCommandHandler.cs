using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUser;

public class GetUserCommandHandler : IRequestHandler<GetUserCommand, User?> {
    private readonly IDataContext _dataContext;

    public GetUserCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public async Task<User?> Handle(GetUserCommand request, CancellationToken cancellationToken) {
        return await _dataContext.Users.FirstOrDefaultAsync(e => e.TelegramId.Equals(request.TelegramId));
    }
}