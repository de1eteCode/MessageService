using MessageService.TelegramService.Common.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.Interfaces;

/// <summary>
/// Шаблон валидатора пользователя
/// </summary>
internal interface IValidator {

    public Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : class, ITelegramRequest;
}