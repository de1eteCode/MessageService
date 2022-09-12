using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по UserName
/// </summary>
public class TelegramLoginValidator : BaseValidator<TelegramLoginAttribute>, ITelegramValidator {

    public Task<TelegramValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return Task.FromResult(TelegramValidatorResult.Anyway);
        }

        foreach (var attr in attributes) {
            if (attr.Login.Equals(user.Username)) {
                return Task.FromResult(TelegramValidatorResult.Allow);
            }
        }

        return Task.FromResult(TelegramValidatorResult.Deny);
    }
}