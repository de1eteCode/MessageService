using Domain.Models;
using MediatR;

namespace Application.ChatGroups.Commands.UpdateChatGroup;
public record UpdateChatGroupCommand : IRequest<ChatGroup> {
    public Guid ChatGroupUID { get; set; }
    public bool IsDeleted { get; set; }
}