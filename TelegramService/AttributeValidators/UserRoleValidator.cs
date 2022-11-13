using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramService.Attributes;
using TelegramService.Commands;
using TelegramService.Enums;
using TelegramService.Interfaces;

namespace TelegramService.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по внутренним ролям
/// </summary>
internal class UserRoleValidator : BaseValidator<UserRoleAttribute>, IValidator {
    private readonly IDataContext _context;

    public UserRoleValidator(IDataContext context) {
        _context = context;
    }

    public async Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return ValidatorResult.Anyway;
        }

        var userModel = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(user.Id));

        if (userModel == null) {
            // нет пользователя, значит нет ролей, значит доступ запрещен
            return ValidatorResult.Deny;
        }

        var role = await _context.Roles.FirstAsync(e => e.UID == userModel.RoleUID);

        foreach (var attr in attributes) {
            if (attr.RoleName.Equals(role.Name)) {
                return ValidatorResult.Allow;
            }
        }

        return ValidatorResult.Deny;
    }
}