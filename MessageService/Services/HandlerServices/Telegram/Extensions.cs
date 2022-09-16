using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageService.Services.HandlerServices.Telegram;

public static class Extensions {
    private const int _maxCharsInMessage = 4096;

    public static async Task<IEnumerable<Message>> SendTextMessageAndSplitIfOverfullAsync(this ITelegramBotClient botClient,
        ChatId chatId,
        string text,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = default,
        bool? allowSendingWithoutReply = default,
        IReplyMarkup? replyMarkup = default,
        CancellationToken cancellationToken = default) {
        var countMessages = (int)Math.Ceiling((decimal)text.Length / _maxCharsInMessage);
        var list = new List<Message>();
        for (int i = 0; i < countMessages; i++) {
            var strToSend = new string(text.Skip(_maxCharsInMessage * i).Take(_maxCharsInMessage).ToArray());
            var msg = await botClient.SendTextMessageAsync(chatId, strToSend, parseMode, entities, disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
            list.Add(msg);
        }
        return list;
    }
}