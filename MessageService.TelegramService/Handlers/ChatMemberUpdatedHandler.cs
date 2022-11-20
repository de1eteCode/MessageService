using MessageService.TelegramService.Common.Interfaces;
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

    public Task HandleUpdate(ChatMemberUpdated handleUpdate, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class MessageHandler : ITelegramUpdateHandler<Message> {

    public Task HandleUpdate(Message handleUpdate, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}