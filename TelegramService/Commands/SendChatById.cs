using Application.Chats.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;
using TelegramService.Extensions;

namespace TelegramService.Commands;

/// <summary>
/// Отправка сообщения в конкретную группу
/// </summary>
[UserRole("Системный администратор")]
internal class SendChatById : BotCommandAction {
    private readonly IMediator _mediator;

    public SendChatById(IMediator mediator) : base("sendchatbyid", "Отправка сообщения в чат по его идентификатору Telegram") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msg = message.Text;
        var chatId = message.Chat.Id;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage();
            return;
        }

        var splitedText = msg.Split(' ');

        if (splitedText.Length < 2) {
            await SendDefaultMessage();
            return;
        }

        var chatIdToSendText = splitedText.First();
        if (long.TryParse(chatIdToSendText, out long chatIdToSend) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{chatIdToSend} не похож на идентификатор группы", cancellationToken: cancellationToken);
            return;
        }

        var chat = await _mediator.Send(new GetChatCommand() { TelegramChatId = chatIdToSend }, cancellationToken);

        if (chat == null) {
            await botClient.SendTextMessageAsync(chatId, $"Не нашел группу с идентификатором {chatIdToSend}", cancellationToken: cancellationToken);
            return;
        }

        var msgToSend = string.Join(" ", splitedText.Skip(1));

        await botClient.SendTextMessageAndSplitIfOverfullAsync(chatIdToSend, msgToSend, cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId, $"Сообщение было отправлено в группу {chat.Name}", cancellationToken: cancellationToken);

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(chatId, "Синтаксис для отправки сообщения в чат: /sendchatbyid [chat id tg] [текст сообщения]", cancellationToken: cancellationToken);
        }
    }
}