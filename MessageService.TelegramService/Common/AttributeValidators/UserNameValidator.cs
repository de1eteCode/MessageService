using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Enums;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по UserName
/// </summary>
internal class UserNameValidator : BaseValidator<TelegramUserNameAttribute>, IValidator {

    public Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : class, ITelegramRequest {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return Task.FromResult(ValidatorResult.Anyway);
        }

        foreach (var attr in attributes) {
            if (attr.UserName.Equals(user.Username)) {
                return Task.FromResult(ValidatorResult.Allow);
            }
        }

        return Task.FromResult(ValidatorResult.Deny);
    }
}