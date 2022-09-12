using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram;

/// <summary>
/// Шаблон валидатора пользователя
/// </summary>
public interface ITelegramValidator {

    public Task<TelegramValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction;
}