using Telegram.Bot;

namespace MessageService.Services.HandlerServices.Telegram;

public interface IUpdateHandler<T> {

    /// <summary>
    /// Обработчик обновлений типа <typeparamref name="T"/>
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="arg"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task HandleUpdateAsync(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken);
}