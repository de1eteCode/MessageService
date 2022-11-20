using MediatR;
using MessageService.TelegramService.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService;

internal class TelegramServiceHost : IHostedService {
    private readonly ILogger<TelegramServiceHost> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly BotClient _botClient;
    private readonly GetUpdatesArgs _updateArgs;
    private bool _isPooling = false;

    public TelegramServiceHost(
        ILogger<TelegramServiceHost> logger,
        IServiceProvider serviceProvider,
        IMediator mediator,
        BotClient botClient) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _botClient = botClient;

        _updateArgs = new GetUpdatesArgs() {
            // Todo:
            //  Set allowed update types
            AllowedUpdates = new List<string>() {
                AllowedUpdate.Message,
                AllowedUpdate.MyChatMember
            },
        };
    }

    #region Interface impl

    public Task StartAsync(CancellationToken cancellationToken) {
        Task.Run(async () => {
            _isPooling = true;
            await SetCommands(cancellationToken);
            await LongPoolingAsync(cancellationToken);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Останавливаем сервис");
        _isPooling = false;
        return Task.CompletedTask;
    }

    #endregion Interface impl

    /// <summary>
    /// Установка комманд бота
    /// </summary>
    private async Task SetCommands(CancellationToken cancellationToken) {
        using (var scope = _serviceProvider.CreateScope()) {
            _logger.LogInformation("Запрашиваем установленные комманды...");
            var currentSetCommands = await _botClient.GetMyCommandsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var definedCommands = scope.ServiceProvider.GetServices<ITelegramRequest>().Select(e => e.BotCommand);

            if (definedCommands.Count() != currentSetCommands.Count() || definedCommands.Any(e => currentSetCommands.Contains(e) == false)) {
                _logger.LogInformation("Удаляем старые команды...");

                var deleteResult = await _botClient.DeleteMyCommandsAsync(
                    scope: new BotCommandScopeDefault(),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (deleteResult) {
                    _logger.LogInformation("Старые команды удалены");
                }
                else {
                    _logger.LogWarning("Старые команды не были удалены");
                }

                _logger.LogInformation("Устанавливаем новые команды...");

                var setResult = await _botClient.SetMyCommandsAsync(
                    commands: definedCommands,
                    scope: new BotCommandScopeDefault(),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (setResult) {
                    _logger.LogInformation("Новые команды установлены");
                }
                else {
                    _logger.LogWarning("Новые команды не были установлены");
                }
            }
            else {
                _logger.LogInformation("Обновление команд не требуется");
            }
        }
    }

    /// <summary>
    /// Обработка входящих обновлений (Long pooling)
    /// </summary>
    private async Task LongPoolingAsync(CancellationToken cancellationToken) {
        try {
            var updates = await _botClient.GetUpdatesAsync(
                limit: _updateArgs.Limit,
                timeout: _updateArgs.Timeout,
                allowedUpdates: _updateArgs.AllowedUpdates,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            while (_isPooling) {
                if (updates.Any()) {
                    Parallel.ForEach(updates, update => HandleUpdate(update));

                    updates = await _botClient.GetUpdatesAsync(
                        offset: updates[^1].UpdateId + 1,
                        limit: _updateArgs.Limit,
                        timeout: _updateArgs.Timeout,
                        allowedUpdates: _updateArgs.AllowedUpdates,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                else {
                    updates = await _botClient.GetUpdatesAsync(
                        limit: _updateArgs.Limit,
                        timeout: _updateArgs.Timeout,
                        allowedUpdates: _updateArgs.AllowedUpdates,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) {
            _logger.LogError("Сервис был остановлен по отзыву токена");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Сервис был остановлен. При получении обновлений возникла ошибка");
        }
        finally {
            _isPooling = false;
        }
    }

    private void HandleUpdate(Update update) {
        using (var scope = _serviceProvider.CreateScope()) {
            var

            throw new NotImplementedException();
        }
    }
}