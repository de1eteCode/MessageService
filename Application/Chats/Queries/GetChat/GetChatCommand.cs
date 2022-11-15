using Domain.Models;
using MediatR;

namespace Application.Chats.Queries.GetChat;
public record GetChatCommand : IRequest<Chat?> {
    public long TelegramChatId { get; set; }
}