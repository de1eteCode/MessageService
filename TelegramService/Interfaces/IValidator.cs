using Telegram.Bot.Types;
using TelegramService.Commands;
using TelegramService.Enums;

namespace TelegramService.Interfaces;

/// <summary>
/// Шаблон валидатора пользователя
/// </summary>
internal interface IValidator {

    public Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction;
}