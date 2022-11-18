using Domain.Models;
using MediatR;

namespace Application.ChatGroups.Queries.GetChatGroup;
public record GetChatGroupCommand : IRequest<ChatGroup?> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
}