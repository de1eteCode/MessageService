using MediatR;
using MessageService.TelegramService.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Handlers;

internal class ChatMemberUpdatedHandler : ITelegramUpdateHandler<ChatMemberUpdated> {
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageHandler> _logger;

    public ChatMemberUpdatedHandler(IServiceProvider serviceProvider, IMediator mediator, ILogger<MessageHandler> logger) {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _logger = logger;
    }

    public Task HandleUpdate(ChatMemberUpdated handleUpdate, UpdateType updateType, CancellationToken cancellationToken) {
        Func<ChatMemberUpdated, CancellationToken, Task> handler = updateType switch {
            UpdateType.MyChatMember => HandleMyChatMemberUpdate,
            UpdateType.ChatMember => HandleChatMemberUpdate,
            _ => throw new NotSupportedException()
        };

        return handler(handleUpdate, cancellationToken);
    }

    private Task HandleMyChatMemberUpdate(ChatMemberUpdated handleUpdate, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    private Task HandleChatMemberUpdate(ChatMemberUpdated handleUpdate, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}