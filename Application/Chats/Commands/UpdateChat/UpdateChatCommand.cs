using Domain.Models;
using MediatR;

namespace Application.Chats.Commands.UpdateChat;
public record UpdateChatCommand : IRequest<Chat> {
    public Guid UID { get; set; }

    public long TelegramChatId { get; set; }

    public string Name { get; set; } = default!;

    public bool IsJoined { get; set; } = default!;

    public string? KickedUserLogin { get; set; }

    public long? KickedUserId { get; set; } = default!;

    public DateTime Time { get; set; } = DateTime.Now;
}