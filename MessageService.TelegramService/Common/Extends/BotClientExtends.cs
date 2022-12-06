using System.Runtime.InteropServices;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.Extends;

internal static class BotClientExtends {
    private const int _maxCharsInMessage = 4096;

    public static async Task<IEnumerable<Message>> SendMessageAndSplitIfOverfullAsync(this BotClient botClient,
        long chatId,
        string text,
        [Optional] int? messageThreadId,
        [Optional] string parseMode,
        [Optional] IEnumerable<MessageEntity>? entities,
        [Optional] bool? disableWebPagePreview,
        [Optional] bool? disableNotification,
        [Optional] bool? protectContent,
        [Optional] int? replyToMessageId,
        [Optional] bool? allowSendingWithoutReply,
        [Optional] ReplyMarkup? replyMarkup,
        [Optional] CancellationToken cancellationToken) {
        if (botClient == null) {
            throw new ArgumentNullException("botClient");
        }

        var countMessages = (int)Math.Ceiling((decimal)text.Length / _maxCharsInMessage);
        var list = new List<Message>();

        for (int i = 0; i < countMessages; i++) {
            var strToSend = new string(text.Skip(_maxCharsInMessage * i).Take(_maxCharsInMessage).ToArray());
            var msg = await botClient.SendMessageAsync(chatId,
                strToSend,
                messageThreadId,
                parseMode,
                entities,
                disableWebPagePreview,
                disableNotification,
                protectContent,
                replyToMessageId,
                allowSendingWithoutReply,
                replyMarkup,
                cancellationToken);
            list.Add(msg);
        }

        return list;
    }
}