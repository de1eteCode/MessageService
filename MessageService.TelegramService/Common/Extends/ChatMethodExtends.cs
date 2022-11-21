using MessageService.TelegramService.Common.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.Extends;

internal static class ChatMethodExtends {

    public static TypeChat GetTypeChat(this Chat chat) {
        if (Enum.TryParse(typeof(TypeChat), chat.Type, ignoreCase: true, out var res)) {
            return (TypeChat)res!;
        }
        else {
            throw new InvalidCastException();
        }
    }
}