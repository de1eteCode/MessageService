using System.Collections.Immutable;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService;

public interface ITelegramHandlerService : IHostedService { }

public class TelegramHandlerService : ITelegramHandlerService {
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;
    private readonly ILogger<TelegramHandlerService> _logger;

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
        if (update.Message is null) {
            _logger.LogInformation("Message is empty");
            return Task.CompletedTask;
        }

        switch (update.Message.Type) {
            case Telegram.Bot.Types.Enums.MessageType.Text:
                var msgtxt = update.Message?.Text?.Split(' ').First() ?? String.Empty;

                if (string.IsNullOrEmpty(msgtxt)) {
                    break;
                }

                var commands = _serviceProvider.GetServices<BotCommandAction>();
                var command = commands.FirstOrDefault(e => e.Command.Equals(msgtxt));

                if (command != null) {
                    update.Message.Text = String.Join(" ", update.Message.Text.Split(' ').Skip(1));
                    command.ExecuteAction(botClient, update, cancellationToken);
                }
                else {
                    if (msgtxt.First().Equals('/')) {
                        _logger.LogInformation("Not supported command: " + update.Message.Text);
                    }
                }

                break;

            default:
                _logger.LogInformation("Not supported type of message: " + update.Message.Type);
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