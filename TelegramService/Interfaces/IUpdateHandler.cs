using Telegram.Bot;

namespace TelegramService.Interfaces;

internal interface IUpdateHandler<T> {

    /// <summary>
    /// Обработчик обновлений типа <typeparamref name="T"/>
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="arg"></param>
    /// <param name="cancellationToken"></param>
    public Task HandleUpdateAsync(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken);
}