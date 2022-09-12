﻿using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.ChatMembers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.Telegram.Handlers;

/// <summary>
/// Обработчик, предназченый для распознования и ответа на действия, которые поступили из чата телеграма
/// </summary>
public class MessageHandler : IUpdateHandler<Message> {
    private readonly ILogger<MessageHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWhoIam _whoIam;
    private readonly IEnumerable<ITelegramValidator> _telegramValidators;

    public MessageHandler(ILogger<MessageHandler> logger, IWhoIam whoIam, IEnumerable<ITelegramValidator> telegramValidators, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _telegramValidators = telegramValidators;
        _whoIam = whoIam;
    }

    private async Task<bool> IsValidAndError<T>(User user, T obj, ITelegramBotClient botClient, ChatId id) where T : BotCommandAction {
        var listOfRes = new List<TelegramValidatorResult>();
        foreach (var validator in _telegramValidators) {
            var curRes = await validator.IsValidAsync(user, obj);
            listOfRes.Add(curRes);
        }

        if (listOfRes.Any(e => e == TelegramValidatorResult.Allow)) {
            return true;
        }

        if (listOfRes.Any(e => e == TelegramValidatorResult.Deny)) {
            await botClient.SendTextMessageAsync(id, "У Вас не достаточно прав для выполнения данной операции");
            return false;
        }

        return true;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        switch (message.Type) {
            case MessageType.Text:
                var msgtxt = message.Text?.Split(' ').First() ?? String.Empty;

                if (string.IsNullOrEmpty(msgtxt) || msgtxt.First().Equals('/') == false) {
                    break;
                }

                // надстройка, чтобы команды работали только в личных сообщениях
                if (message.Chat.Type != ChatType.Private) {
                    break;
                }

                var commands = _serviceProvider.GetServices<BotCommandAction>();
                var command = commands.FirstOrDefault(e => e.Command.Equals(msgtxt));

                if (command != null) {
                    if (await IsValidAndError(message.From!, command, botClient, message.Chat.Id) == false) {
                        break;
                    }

                    message.Text = String.Join(" ", message.Text!.Split(' ').Skip(1));
                    await command.ExecuteActionAsync(botClient, message, cancellationToken);
                }
                else {
                    _logger.LogInformation("Not supported command: " + message.Text);
                }

                break;

            case MessageType.GroupCreated:
            case MessageType.ChatMembersAdded:
                if (message.Type == MessageType.GroupCreated) { // <- if тут для того, чтоб избежать ошибки компиляции CS0163: Управление не может передаваться вниз от одной метки case к другой
                    // При создании группы, в свойстве NewMembers - null. Необходимо добавить хотя бы бота, как нового участника, чтоб запомнил чат
                    message.NewChatMembers = new User[] { await _whoIam.GetMeAsync() };
                }

                await _serviceProvider.GetService<RememberChat>()!.ExecuteActionAsync(message);
                break;

            case MessageType.ChatMemberLeft:
                await _serviceProvider.GetService<ForgetChat>()!.ExecuteActionAsync(message);
                break;

            default:
                _logger.LogInformation("Not supported type of message: " + message.Type);
                break;
        }
    }
}