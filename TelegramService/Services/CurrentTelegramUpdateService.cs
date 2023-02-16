using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramService.Interfaces;

namespace TelegramService.Services;

internal class CurrentTelegramUpdateService : ICurrentTelegramUpdate {
    private Update? _update;

    public Update Update => _update ?? throw new NullReferenceException();

    public long? GetFromUserId() {
        if (_update == null) {
            return null;
        }

        User? from = _update.Type switch {
            UpdateType.Message => _update.Message?.From,
            UpdateType.InlineQuery => _update.InlineQuery?.From,
            UpdateType.ChosenInlineResult => _update.ChosenInlineResult?.From,
            UpdateType.CallbackQuery => _update.CallbackQuery?.From,
            UpdateType.EditedMessage => _update.EditedMessage?.From,
            UpdateType.ChannelPost => _update.ChannelPost?.From,
            UpdateType.EditedChannelPost => _update.EditedChannelPost?.From,
            UpdateType.ShippingQuery => _update.ShippingQuery?.From,
            UpdateType.PreCheckoutQuery => _update.PreCheckoutQuery?.From,
            UpdateType.MyChatMember => _update.MyChatMember?.From,
            UpdateType.ChatMember => _update.ChatMember?.From,
            UpdateType.ChatJoinRequest => _update.ChatJoinRequest?.From,
            _ => null,
        };

        return from?.Id ?? null;
    }

    public void SetUpdate(Update update) {
        _update = update;
    }
}