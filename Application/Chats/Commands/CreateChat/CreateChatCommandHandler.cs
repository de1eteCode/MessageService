using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Chats.Commands.CreateChat;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Chat> {
    private readonly IDataContext _dataContext;

    public CreateChatCommandHandler(IDataContext dataContext) {
        _dataContext = dataContext;
    }

    public Task<Chat> Handle(CreateChatCommand request, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}