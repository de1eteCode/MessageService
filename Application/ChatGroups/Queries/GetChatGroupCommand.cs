using Domain.Models;
using MediatR;

namespace Application.ChatGroups.Queries;
public record GetChatGroupCommand : IRequest<ChatGroup?> {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
}