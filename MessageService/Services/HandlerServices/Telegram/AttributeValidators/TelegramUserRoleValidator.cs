using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using DataLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по внутренним ролям
/// </summary>
public class TelegramUserRoleValidator : BaseValidator<TelegramUserRoleAttribute>, ITelegramValidator {
    private readonly IDatabaseService<DataContext> _dbService;

    public TelegramUserRoleValidator(IDatabaseService<DataContext> dbService) {
        _dbService = dbService;
    }

    public async Task<TelegramValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : BotCommandAction {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return TelegramValidatorResult.Anyway;
        }

        var context = _dbService.GetDBContext();

        var userModel = await context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(user.Id));

        if (userModel == null) {
            // нет пользователя, значит нет ролей, значит доступ запрещен
            return TelegramValidatorResult.Deny;
        }

        var role = await context.Roles.FirstAsync(e => e.UID == userModel.RoleUID);

        foreach (var attr in attributes) {
            if (attr.RoleName.Equals(role.Name)) {
                return TelegramValidatorResult.Allow;
            }
        }

        return TelegramValidatorResult.Deny;
    }
}