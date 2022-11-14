using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramService.Attributes;
using Application.Common.Interfaces;
using TelegramService.Extensions;

namespace TelegramService.Commands;

/// <summary>
/// Отправка сообщения в конкретную группу
/// </summary>
[UserRole("Системный администратор")]
internal class SendChatById : BotCommandAction {
    private readonly IDataContext _context;

    public SendChatById(IDataContext dataContext) : base("sendchatbyid", "Отправка сообщения в чат по его идентификатору Telegram") {
        _context = dataContext;
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
            await botClient.SendTextMessageAsync(chatId, $"{chatIdToSend} не похож на идентификатор группы");
            return;
        }

        var chat = _context.Chats.FirstOrDefault(e => e.TelegramChatId.Equals(chatIdToSend));

        if (chat == null) {
            await botClient.SendTextMessageAsync(chatId, $"Не нашел группу с идентификатором {chatIdToSend}");
            return;
        }

        var msgToSend = string.Join(" ", splitedText.Skip(1));

        await botClient.SendTextMessageAndSplitIfOverfullAsync(chatIdToSend, msgToSend);
        await botClient.SendTextMessageAsync(chatId, $"Сообщение было отправлено в группу {chat.Name}");

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(chatId, "Синтаксис для отправки сообщения в чат: /sendchatbyid [chat id tg] [текст сообщения]");
        }
    }
}