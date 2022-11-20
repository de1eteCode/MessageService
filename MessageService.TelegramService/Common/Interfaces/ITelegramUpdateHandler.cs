using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramUpdateHandler<TUpdate> {

    public Task HandleUpdate(TUpdate handleUpdate, CancellationToken cancellationToken);
}