using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramService.Commands;
using TelegramService.Interfaces;
using TelegramService.Models;
using TelegramService.Services;

namespace TelegramService;

public class TelegramHostedService : IHostedService, IWhoIam {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramHostedService> _logger;
    private readonly TelegramBotClient _telegramClient;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ReceiverOptions _receiverOptions;

    private readonly Dictionary<UpdateType, Func<ITelegramBotClient, Update, CancellationToken, IServiceProvider, Task>> _supportedUpdates;

    private User _meUser = default!;

    public TelegramHostedService(ILogger<TelegramHostedService> logger, IOptionsMonitor<TelegramSettings> optionsMonitor, IServiceScopeFactory scopeFactory) {
        _supportedUpdates = new() {
            { UpdateType.Message, (client, update, ct, serviceProvider) => GetHandlerForMessageType(client, update.Message, ct, serviceProvider) },
            { UpdateType.MyChatMember, (client, update, ct, serviceProvider) => GetHandlerForMessageType(client, update.MyChatMember, ct, serviceProvider) }
        };

        _logger = logger;

        _scopeFactory = scopeFactory;
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
        using (var scope = _scopeFactory.CreateScope()) {
            var commandsBot = scope.ServiceProvider.GetServices<BotCommandAction>();
            _telegramClient.SetMyCommandsAsync(commandsBot, scope: BotCommandScope.AllPrivateChats(), cancellationToken: _tokenSource.Token);
        }

        cancellationToken.ThrowIfCancellationRequested();

        _telegramClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, _tokenSource.Token);

        if (cancellationToken.IsCancellationRequested) {
            _tokenSource.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
        }

        _logger.LogInformation($"Запущен {nameof(TelegramService)}");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Остановка сервиса
    /// Метод, предоставляемый <see cref="IHostedService"/>
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) {
        _tokenSource.Cancel();
        _logger.LogInformation($"Остановлен {nameof(TelegramService)}");

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
        using (var scope = _scopeFactory.CreateScope()) {
            var tgUpdateService = scope.ServiceProvider.GetService<ICurrentTelegramUpdate>() as CurrentTelegramUpdateService ?? throw new Exception();
            tgUpdateService.SetUpdate(update);

            if (_supportedUpdates.TryGetValue(update.Type, out var handler)) {
                cancellationToken.ThrowIfCancellationRequested();
                await handler(botClient, update, cancellationToken, scope.ServiceProvider).ConfigureAwait(false);
            }
            else {
                _logger.LogError($"Handle update unsupported type: {update.Type}");
            }
        }
    }

    /// <summary>
    /// Получение хендлера для сообщения типа <typeparamref name="T"/>
    /// </summary>
    private Task GetHandlerForMessageType<T>(ITelegramBotClient botClient, T arg, CancellationToken cancellationToken, IServiceProvider scope) {
        var service = scope.GetService<IUpdateHandler<T>>() ?? throw new Exception($"Not found service typeof {typeof(IUpdateHandler<T>)}.");
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