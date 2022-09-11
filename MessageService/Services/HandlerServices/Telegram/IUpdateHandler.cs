using Telegram.Bot;

namespace MessageService.Services.HandlerServices.Telegram;

public interface IUpdateHandler<T> {

    /// <summary>
    ///
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="arg"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task HandleUpdateAsync(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken);
}