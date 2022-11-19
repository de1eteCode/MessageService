using Domain.Models;
using MediatR;

namespace Application.Chats.Queries.GetChat;

public record GetChatsCommand : IRequest<IEnumerable<Chat>> {
}