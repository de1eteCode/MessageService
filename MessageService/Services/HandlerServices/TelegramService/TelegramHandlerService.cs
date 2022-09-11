using MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.TelegramService;

public interface ITelegramHandlerService : IHostedService {
    public bool IsStarted { get; }

    /// <summary>
    /// Информация о боте
    /// </summary>
    public Task<User> GetMeAsync();
}

public class TelegramHandlerService : ITelegramHandlerService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramHandlerService> _logger;
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;

    private readonly Dictionary<UpdateType, Func<ITelegramBotClient, Update, CancellationToken, Task>> _supportedUpdates;

    private User _meUser = default!;

    public bool IsStarted { get; private set; }

    public TelegramHandlerService(IConfiguration configuration, ILogger<TelegramHandlerService> logger, IServiceProvider serviceProvider) {
        _supportedUpdates = new() {
            { UpdateType.Message, (client, update, ct) => HandleInHandler(client, update.Message, ct) },
        };

        _logger = logger;

        _serviceProvider = serviceProvider;
        _receiverOptions = new ReceiverOptions() {
            AllowedUpdates = _supportedUpdates.Keys.ToArray()
        };

        _tokenSource = new CancellationTokenSource();
        _telegramClient = new TelegramBotClient(configuration["TelegramToken"]);
    }

    /// <summary>
    /// Запуск сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken) {
        _telegramClient.SetMyCommandsAsync(_serviceProvider.GetServices<BotCommandAction>(), cancellationToken: _tokenSource.Token);
        _telegramClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, _tokenSource.Token);
        _logger.LogInformation($"Запущен {nameof(TelegramHandlerService)}");
        IsStarted = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Остановка сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) {
        _tokenSource.Cancel();
        _logger.LogInformation($"Остановлен {nameof(TelegramHandlerService)}");
        IsStarted = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обработка входящего сообщения от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="update"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        if (_supportedUpdates.TryGetValue(update.Type, out var handler)) {
            await handler(botClient, update, cancellationToken);
        }
        else {
            _logger.LogError($"Handle update unsupported type: {update.Type}");
        }
    }

    /// <summary>
    /// Получение хендлера для сообщения типа <typeparamref name="T"/>
    /// </summary>
    private Task HandleInHandler<T>(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken) {
        var service = _serviceProvider.GetService<IUpdateHandler<T>>() ?? throw new Exception($"Not found service typeof {typeof(IUpdateHandler<T>)}.");
        return service.HandleUpdateAsync(botClient, arg, cancellationToken);
    }

    /// <summary>
    /// Обработка ошибок от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="exception">Исключение</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
        var msg = exception switch {
            ApiRequestException apiRequestException => $"Telegram api error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _ => $"Unknow error: ({exception.GetType()}) {exception.Message}"
        };

        _logger.LogError(msg);

        return Task.CompletedTask;
    }

    public async Task<User> GetMeAsync() {
        if (_meUser == null) {
            _meUser = await _telegramClient.GetMeAsync();
        }

        return _meUser;
    }
}