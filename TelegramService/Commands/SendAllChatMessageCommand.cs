using Application.Chats.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Рассылка сообщений по всем чатам
/// </summary>
[UserRole("Системный администратор")]
internal class SendAllChatMessageCommand : BotCommandAction {
    private readonly IMediator _mediator;
    private readonly ILogger<SendAllChatMessageCommand> _logger;

    public SendAllChatMessageCommand(IMediator mediator, ILogger<SendAllChatMessageCommand> logger) : base("sendallchat", "Отправка сообщения во все чаты") {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgToSend = message.Text;

        if (string.IsNullOrEmpty(msgToSend)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщение для отправки было пусто. Используйте команду /sendallchat текст сообщения");
            return;
        }

        var chats = await _mediator.Send(new GetChatsCommand());

        var chatSended = 0;

        var tasksToSend = chats.Where(e => e.IsJoined).Select(e => Task.Run(async () => {
            try {
                var msg = await botClient.SendTextMessageAsync(e.TelegramChatId, msgToSend!);
                if (msg != null) {
                    Interlocked.Increment(ref chatSended);
                }
            }
            catch (Exception e) {
                _logger.LogError(e, "Error send");
            }
        }));

        await Task.WhenAll(tasksToSend);

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Сообщение отправлено в {chatSended} чатов");
    }
}