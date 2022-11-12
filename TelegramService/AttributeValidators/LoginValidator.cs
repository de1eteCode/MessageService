using Telegram.Bot.Types;
using TelegramService.Attributes;
using TelegramService.Commands;
using TelegramService.Enums;
using TelegramService.Interfaces;

namespace TelegramService.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по UserName
/// </summary>
internal class TelegramLoginValidator : BaseValidator<LoginAttribute>, IValidator {

    public Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return Task.FromResult(ValidatorResult.Anyway);
        }

        foreach (var attr in attributes) {
            if (attr.Login.Equals(user.Username)) {
                return Task.FromResult(ValidatorResult.Allow);
            }
        }

        return Task.FromResult(ValidatorResult.Deny);
    }
}