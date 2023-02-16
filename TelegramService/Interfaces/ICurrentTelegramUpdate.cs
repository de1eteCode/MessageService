using Telegram.Bot.Types;

namespace TelegramService.Interfaces;

internal interface ICurrentTelegramUpdate {
    public Update Update { get; }

    public long? GetFromUserId();
}