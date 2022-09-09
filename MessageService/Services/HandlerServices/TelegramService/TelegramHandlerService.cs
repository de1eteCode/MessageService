using System.Collections.Immutable;
using MessageService.Services.HandlerServices.TelegramService.Handlers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService;

public interface ITelegramHandlerService : IHostedService { }

public class TelegramHandlerService : ITelegramHandlerService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramHandlerService> _logger;
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;

    public TelegramHandlerService(IConfiguration configuration, ILogger<TelegramHandlerService> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _receiverOptions = new ReceiverOptions();
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
        return Task.CompletedTask;
    }

    /// <summary>
    /// Остановка сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) {
        _tokenSource.Cancel();
        _logger.LogInformation($"Остановлен {nameof(TelegramHandlerService)}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обработка входящего сообщения от telegram бота
    /// </summary>
    /// <param name="botClient">Клиент telegram бота</param>
    /// <param name="update"></param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        switch (update.Type) {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
                var obj = _serviceProvider.GetService<CommandHandler>();
                return obj.HandleUpdateAsync(botClient, update, cancellationToken);

            case Telegram.Bot.Types.Enums.UpdateType.InlineQuery:
            case Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult:
            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
            case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
            case Telegram.Bot.Types.Enums.UpdateType.ChannelPost:
            case Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost:
            case Telegram.Bot.Types.Enums.UpdateType.ShippingQuery:
            case Telegram.Bot.Types.Enums.UpdateType.PreCheckoutQuery:
            case Telegram.Bot.Types.Enums.UpdateType.Poll:
            case Telegram.Bot.Types.Enums.UpdateType.PollAnswer:
            case Telegram.Bot.Types.Enums.UpdateType.MyChatMember:
            case Telegram.Bot.Types.Enums.UpdateType.ChatMember:
            case Telegram.Bot.Types.Enums.UpdateType.ChatJoinRequest:
            case Telegram.Bot.Types.Enums.UpdateType.Unknown:
            default:
                _logger.LogError($"Обновления типа {update.Type} не поддерживаются");
                break;
        }

        return Task.CompletedTask;
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