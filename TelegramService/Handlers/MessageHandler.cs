﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramService.Commands;
using TelegramService.Enums;
using TelegramService.Interfaces;

namespace TelegramService.Handlers;

/// <summary>
/// Обработчик, предназченый для распознования и ответа на действия, которые поступили из чата телеграма
/// </summary>
internal class MessageHandler : IUpdateHandler<Message> {
    private readonly ILogger<MessageHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IValidator> _telegramValidators;

    public MessageHandler(ILogger<MessageHandler> logger, IEnumerable<IValidator> telegramValidators, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _telegramValidators = telegramValidators;
    }

    private async Task<bool> IsValidAndError<T>(User user, T obj, ITelegramBotClient botClient, ChatId id)
        where T : BotCommandAction {
        var listOfRes = new List<ValidatorResult>();
        foreach (var validator in _telegramValidators) {
            var curRes = await validator.IsValidAsync(user, obj);
            listOfRes.Add(curRes);
        }

        if (listOfRes.Any(e => e == ValidatorResult.Allow)) {
            return true;
        }

        if (listOfRes.Any(e => e == ValidatorResult.Deny)) {
            await botClient.SendTextMessageAsync(id, "У Вас не достаточно прав для выполнения данной операции");
            return false;
        }

        return true;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        // надстройка, чтобы команды работали только в личных сообщениях
        if (message.Chat.Type != ChatType.Private) {
            return;
        }

        switch (message.Type) {
            case MessageType.Text:
                var msgtxt = message.Text?.Split(' ').First() ?? String.Empty;

                if (string.IsNullOrEmpty(msgtxt) || msgtxt.First().Equals('/') == false) {
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

                break;

            default:
                _logger.LogInformation("Not supported type of message: " + message.Type);
                break;
        }
    }
}