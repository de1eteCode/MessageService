using MessageService.Datas;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
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

        var userModel = await context.Users.FirstOrDefaultAsync(e => e.Id!.Equals(user.Id.ToString()));

        if (userModel == null) {
            // нет пользователя, значит нет ролей, значит доступ запрещен
            return TelegramValidatorResult.Deny;
        }

        var role = await context.Roles.FirstAsync(e => e.RoleId == userModel.RoleId);

        foreach (var attr in attributes) {
            if (attr.RoleName.Equals(role.RoleName)) {
                return TelegramValidatorResult.Allow;
            }
        }

        return TelegramValidatorResult.Deny;
    }
}