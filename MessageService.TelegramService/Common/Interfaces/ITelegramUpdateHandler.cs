using Telegram.BotAPI;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramUpdateHandler<TUpdate> {

    public Task HandleUpdate(TUpdate handleUpdate, UpdateType updateType, CancellationToken cancellationToken);
}