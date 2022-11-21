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
    private readonly BotClient _botClient;
    private readonly GetUpdatesArgs _updateArgs;
    private bool _isPooling = false;

    /// <summary>
    /// Временное решение, т.к. необходимо реализовать унифицированный механизм добавления обработчиков для каждого типа из <see cref="UpdateType"/>
    /// </summary>
    private readonly Dictionary<UpdateType, Func<Update, IServiceScope, Task>> _supportUpdates;

    public TelegramServiceHost(
        ILogger<TelegramServiceHost> logger,
        IServiceProvider serviceProvider,
        BotClient botClient) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botClient = botClient;

        _supportUpdates = new() {
            { UpdateType.Message, (update, scope) => GetUpdateHandler(update.Message, UpdateType.Message, scope) },
            { UpdateType.MyChatMember, (update, scope) => GetUpdateHandler(update.MyChatMember, UpdateType.MyChatMember, scope) },
            { UpdateType.ChatMember, (update, scope) => GetUpdateHandler(update.ChatMember, UpdateType.ChatMember, scope) },
        };

        var allowDict = new Dictionary<UpdateType, string>() {
            { UpdateType.Message, AllowedUpdate.Message },
            { UpdateType.EditedMessage, AllowedUpdate.EditedMessage },
            { UpdateType.ChannelPost, AllowedUpdate.ChannelPost },
            { UpdateType.EditedChannelPost, AllowedUpdate.EditedChannelPost },
            { UpdateType.InlineQuery, AllowedUpdate.InlineQuery },
            { UpdateType.ChosenInlineResult, AllowedUpdate.ChosenInlineResult },
            { UpdateType.CallbackQuery, AllowedUpdate.CallbackQuery },
            { UpdateType.ShippingQuery, AllowedUpdate.ShippingQuery },
            { UpdateType.PreCheckoutQuery, AllowedUpdate.PreCheckoutQuery },
            { UpdateType.Poll, AllowedUpdate.Poll },
            { UpdateType.PollAnswer, AllowedUpdate.PollAnswer },
            { UpdateType.MyChatMember, AllowedUpdate.MyChatMember },
            { UpdateType.ChatMember, AllowedUpdate.ChatMember },
            { UpdateType.ChatJoinRequest, AllowedUpdate.ChatJoinRequest }
        };

        _updateArgs = new GetUpdatesArgs() {
            AllowedUpdates = _supportUpdates.Select(e => allowDict[e.Key])
        };
    }

    #region Interface impl

    public Task StartAsync(CancellationToken cancellationToken) {
        Task.Run(async () => {
            _isPooling = true;
            await SetCommands(cancellationToken);
            _logger.LogInformation("Запускаем сервис...");
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
                    Parallel.ForEach(updates, async update => await HandleUpdate(update).ConfigureAwait(false));

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

    #region Handle update

    private async Task HandleUpdate(Update update) {
        using (var scope = _serviceProvider.CreateScope()) {
            if (_supportUpdates.TryGetValue(update.Type, out var handler)) {
                await handler(update, scope).ConfigureAwait(false);
            }
        }
    }

    private Task GetUpdateHandler<T>(T arg, UpdateType updateType, IServiceScope scope) {
        var service = scope.ServiceProvider.GetService<ITelegramUpdateHandler<T>>() ?? throw new Exception($"Not found service type of {typeof(ITelegramUpdateHandler<T>)}");
        return service.HandleUpdate(arg, updateType, default);
    }

    #endregion Handle update
}