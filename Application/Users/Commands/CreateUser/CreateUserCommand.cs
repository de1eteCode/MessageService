using Domain.Models;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<User> {
    public string Name { get; set; } = default!;
    public long TelegramId { get; set; }
    public Guid RoleUID { get; set; }
}