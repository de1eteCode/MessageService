using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService;

public interface ITelegramHandlerService : IHandlerService { }

public class TelegramHandlerService : ITelegramHandlerService {
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;
    private readonly ILogger<TelegramHandlerService> _logger;

    public TelegramHandlerService(IConfiguration configuration, ILogger<TelegramHandlerService> logger) {
        _logger = logger;
        _receiverOptions = new ReceiverOptions();
        _tokenSource = new CancellationTokenSource();
        _telegramClient = new TelegramBotClient(configuration["TelegramToken"]);

        _telegramClient.SetMyCommandsAsync(new List<BotCommandAction>(), cancellationToken: _tokenSource.Token);
    }

    public void StartListener() {
        _telegramClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, _tokenSource.Token);
    }

    public void StopListener() {
        _tokenSource.Cancel();
    }

    /// <summary>
    /// Обработка входящего сообщения от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="update"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        // определение операции
        // вызов обработчика для нужной команды
        throw new NotImplementedException();
    }

    /// <summary>
    /// Обработка ошибок от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="exception">Исключение</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
        var msg = exception switch {
            ApiRequestException apiRequestException => $"Telegram api error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _ => $"Unknow error: ({exception.GetType()}) {exception.Message}"
        };

        _logger.LogError(msg);

        return Task.CompletedTask;
    }
}