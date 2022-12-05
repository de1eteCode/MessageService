using MediatR;
using MessageService.TelegramService.Commands;
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
    private readonly IMediator _mediator;
    private readonly ILogger<MessageHandler> _logger;

    public ChatMemberUpdatedHandler(IMediator mediator, ILogger<MessageHandler> logger) {
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
        _logger.LogInformation("Добавление чата: " + handleUpdate.Chat.Title);
        return _mediator.Send(new RememberChatCommand() { ChatMemberUpdate = handleUpdate });
    }

    private Task HandleChatMemberUpdate(ChatMemberUpdated handleUpdate, CancellationToken cancellationToken) {
        _logger.LogInformation("Удаление чата: " + handleUpdate.Chat.Title);
        return _mediator.Send(new ForgetChatCommand() { ChatMemberUpdate = handleUpdate });
    }
}