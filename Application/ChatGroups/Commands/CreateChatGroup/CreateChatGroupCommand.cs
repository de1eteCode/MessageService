using Domain.Models;
using MediatR;

namespace Application.ChatGroups.Commands.CreateChatGroup;
public record CreateChatGroupCommand : IRequest<ChatGroup> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
}