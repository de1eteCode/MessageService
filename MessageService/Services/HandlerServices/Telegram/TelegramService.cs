using System.Diagnostics;
using MessageService.Models;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using Microsoft.Extensions.Options;
using RPSLimitTelegramBotLibrary.Telegram;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.Telegram;

public interface ITelegramService : IHostedService {
    public TelegramBotClient TelegramBot { get; }
    public bool IsStarted { get; }
}

public class TelegramService : ITelegramService, IWhoIam, ITelegramSenderMessage {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramService> _logger;
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;

    private readonly Dictionary<UpdateType, Func<ITelegramBotClient, Update, CancellationToken, Task>> _supportedUpdates;

    private User _meUser = default!;

    public bool IsStarted { get; private set; }
    public TelegramBotClient TelegramBot => _telegramClient;

    public TelegramService(ILogger<TelegramService> logger, IOptionsMonitor<TelegramSettings> optionsMonitor, IServiceProvider serviceProvider) {
        _supportedUpdates = new() {
            { UpdateType.Message, (client, update, ct) => GetHandlerForMessageType(client, update.Message, ct) },
            { UpdateType.MyChatMember, (client, update, ct) => GetHandlerForMessageType(client, update.MyChatMember, ct) }
        };

        _logger = logger;

        _serviceProvider = serviceProvider;
        _receiverOptions = new ReceiverOptions() {
            AllowedUpdates = _supportedUpdates.Keys.ToArray()
        };

        var options = optionsMonitor.CurrentValue;

        _tokenSource = new CancellationTokenSource();
        _telegramClient = new TelegramBotClientLimit(options.Token) {
            Rate = options.LimitRequests,
            TimeForOperation = TimeSpan.FromSeconds(1)
        };
    }

    #region Service method

    /// <summary>
    /// Запуск сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken) {
        var commandsBot = _serviceProvider.GetServices<BotCommandAction>();
        _telegramClient.SetMyCommandsAsync(commandsBot, cancellationToken: _tokenSource.Token);
        _telegramClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, _tokenSource.Token);
        _logger.LogInformation($"Запущен {nameof(TelegramService)}");
        IsStarted = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Остановка сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) {
        _tokenSource.Cancel();
        _logger.LogInformation($"Остановлен {nameof(TelegramService)}");
        IsStarted = false;
        return Task.CompletedTask;
    }

    #endregion Service method

    /// <summary>
    /// Обработка входящего сообщения от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="update"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        if (_supportedUpdates.TryGetValue(update.Type, out var handler)) {
            await handler(botClient, update, cancellationToken).ConfigureAwait(false);
        }
        else {
            _logger.LogError($"Handle update unsupported type: {update.Type}");
        }
    }

    /// <summary>
    /// Получение хендлера для сообщения типа <typeparamref name="T"/>
    /// </summary>
    private Task GetHandlerForMessageType<T>(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken) {
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

    public async Task SendMessageAsync(string message, Datas.Models.Chat chat, int @try = 1) {
        if (string.IsNullOrEmpty(message)) {
            return;
        }

        for (; @try < 3; @try++) {
            var resMsg = await _telegramClient.SendTextMessageAsync(new ChatId(chat.ChatId ?? throw new ArgumentNullException(nameof(chat))), message);
            if (resMsg != null) {
                break;
            }
        }
    }
}