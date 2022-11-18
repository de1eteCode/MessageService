using Application.Roles.Queries.GetRoles;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.GetUser;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Команда изменения пользователя
/// </summary>
internal class ChangeUserCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public ChangeUserCommand(IMediator mediator) : base("changeuser", "Изменение пользователя") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat!.Id;
        var msgText = message.Text!;

        if (string.IsNullOrWhiteSpace(msgText)) {
            await SendDefaultMessage();
            return;
        }

        var splitedText = msgText.Split(new char[] { ' ' }, 2);

        if (splitedText.Length != 2) {
            await SendDefaultMessage();
            return;
        }

        var idTelegramStr = splitedText.First();

        if (long.TryParse(idTelegramStr, out long idTelegram) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"{idTelegramStr} не похож на идентификатор пользователя Telegram", cancellationToken: cancellationToken);
            return;
        }

        // проверка на наличие такого пользователя
        var userForChange = await _mediator.Send(new GetUserCommand() { TelegramId = idTelegram }, cancellationToken);

        if (userForChange == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {idTelegram} не найден", cancellationToken: cancellationToken);
            return;
        }

        var roleId = splitedText.Last();

        if (int.TryParse(roleId, out int roleIdNum) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил", cancellationToken: cancellationToken);
        }

        var selectedRoleUser = await _mediator.Send(new GetRoleByAlternativeIdCommand() { AlternativeId = roleIdNum }, cancellationToken);

        if (selectedRoleUser == null) {
            await botClient.SendTextMessageAsync(privateChatId, "Я не нашел роль под id " + roleIdNum);
            return;
        }

        userForChange = await _mediator.Send(new UpdateUserCommand() {
            UserUID = userForChange.UID,
            RoleUID = selectedRoleUser.UID,
        }, cancellationToken);

        await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {userForChange.Name} был успешно изменен", cancellationToken: cancellationToken);

        return;

        async Task SendDefaultMessage() {
            var roles = await _mediator.Send(new GetRolesCommand(), cancellationToken);

            var rolesStrCollection = roles
                .OrderBy(e => e.AlternativeId)
                .Select(e => string.Format("{0}. {1}", e.AlternativeId, e.Name))
                .ToList();

            await botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис изменения пользователя: /changeuser [tg user id] [id роли]\n" +
                "Доступные роли:\n" +
                String.Join("\n", rolesStrCollection),
                cancellationToken: cancellationToken);
        }
    }
}