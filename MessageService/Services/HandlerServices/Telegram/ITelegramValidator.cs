using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram;

public interface ITelegramValidator {

    public Task<TelegramValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction;
}