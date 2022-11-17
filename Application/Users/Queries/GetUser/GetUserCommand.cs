using Domain.Models;
using MediatR;

namespace Application.Users.Queries.GetUser;

public record GetUserCommand : IRequest<User?> {
    public long TelegramId { get; set; }
}