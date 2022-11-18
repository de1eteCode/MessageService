using Application.Common.Interfaces;
using Application.Roles.Queries.GetRoles;
using Application.Users.Commands.CreateUser;
using Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Добавление нового пользователя в БД <see cref="DataContext"/>
/// </summary>
[UserRole("Системный администратор")]
[Login("de1alex")]
internal class AddUserCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public AddUserCommand(IMediator mediator) : base("adduser", "Добавление пользователя") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgText = message.Text!;
        var chatId = message.Chat.Id;

        // фильтрация на пустое сообщение
        if (string.IsNullOrEmpty(msgText)) {
            await SendDefaultMsg();
            return;
        }

        // фильтрация на количество аргументов
        var splitedText = msgText.Split(' ');

        if (splitedText.Length < 3) {
            await SendDefaultMsg();
            return;
        }

        var idTelegramStr = splitedText[(int)PositionArgs.TelegramId];

        if (long.TryParse(idTelegramStr, out long idTelegram) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{idTelegramStr} не похож на идентификатор пользователя Telegram");
            return;
        }

        // проверка на наличие такого пользователя
        var addedUser = await _mediator.Send(new GetUserCommand() { TelegramId = idTelegram });

        if (addedUser != null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {addedUser.Name} был ранее добавлен");
            return;
        }

        var roleId = splitedText[(int)PositionArgs.RoleId];

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = await _mediator.Send(new GetRoleByAlternativeIdCommand() { AlternativeId = roleIdNum });

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(chatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            var newUser = await _mediator.Send(new CreateUserCommand() {
                TelegramId = idTelegram,
                Name = String.Join(" ", splitedText.Skip((int)PositionArgs.Name)),
                RoleUID = selectedRoleUser.UID
            });

            await botClient.SendTextMessageAsync(chatId, $"Пользователь {newUser.Name} был успешно добавлен с ролью {newUser.Role.Name}");
        }
        else {
            await botClient.SendTextMessageAsync(chatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил");
        }

        return;

        async Task SendDefaultMsg() {
            var roles = await _mediator.Send(new GetRolesCommand());

            var rolesStrCollection = roles
                .OrderBy(e => e.AlternativeId)
                .Select(e => string.Format("{0}. {1}", e.AlternativeId, e.Name))
                .ToList();

            await botClient.SendTextMessageAsync(chatId,
                "Синтаксис для добавления пользователя: /adduser [id роль] [id telegram] [Имя]\n" +
                "Доступные роли:\n" +
                String.Join("\n", rolesStrCollection));
        }
    }

    private enum PositionArgs : int {
        RoleId = 0,
        TelegramId = 1,
        Name = 2
    }
}