using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.ChatMembers;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;
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

    public MessageHandler(ILogger<MessageHandler> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
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
                else {
                    _logger.LogInformation("Not supported command: " + message.Text);
                }

                break;

            case MessageType.GroupCreated:
            case MessageType.ChatMembersAdded:
                await _serviceProvider.GetService<RememberChat>()!.Execute(message);
                break;

            case MessageType.ChatMemberLeft:
                await _serviceProvider.GetService<ForgetChat>()!.Execute(message);
                break;

            default:
                _logger.LogInformation("Not supported type of message: " + message.Type);
                break;
        }
    }
}