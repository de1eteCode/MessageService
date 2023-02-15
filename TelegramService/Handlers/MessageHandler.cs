using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramService.Commands;
using TelegramService.Interfaces;

namespace TelegramService.Handlers;

/// <summary>
/// Обработчик, предназченый для распознования и ответа на действия, которые поступили из чата телеграма
/// </summary>
internal class MessageHandler : IUpdateHandler<Message> {
    private readonly ILogger<MessageHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MessageHandler(ILogger<MessageHandler> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
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