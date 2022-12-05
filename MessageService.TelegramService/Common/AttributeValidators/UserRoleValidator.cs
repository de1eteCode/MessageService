using Application.Users.Queries;
using MediatR;
using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Enums;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.AttributeValidators;

/// <summary>
/// Валидация телеграмм пользователя по внутренним ролям
/// </summary>
internal class UserRoleValidator : BaseValidator<TelegramUserRoleAttribute>, IValidator {
    private readonly IMediator _mediator;

    public UserRoleValidator(IMediator mediator) {
        _mediator = mediator;
    }

    public async Task<ValidatorResult> IsValidAsync<T>(User user, T obj)
        where T : class, ITelegramRequest {
        var attributes = GetAttributes(obj);

        if (attributes.Any() == false) {
            // атрибутов нет
            return ValidatorResult.Anyway;
        }
        var userModel = await _mediator.Send(new GetUserCommand() { TelegramId = user.Id });

        if (userModel == null) {
            // нет пользователя, значит нет ролей, значит доступ запрещен
            return ValidatorResult.Deny;
        }

        foreach (var attr in attributes) {
            if (attr.RoleName.Equals(userModel.Role.Name)) {
                return ValidatorResult.Allow;
            }
        }

        return ValidatorResult.Deny;
    }
}