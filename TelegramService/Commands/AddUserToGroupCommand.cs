using Application.Groups.Queries;
using Application.UserGroups.Commands;
using Application.Users.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Добавление пользователя в группу
/// </summary>
internal class AddUserToGroupCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public AddUserToGroupCommand(IMediator mediator) : base("addusertogroup", "Добавление пользователя в группу") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgText = message.Text!;
        var chatId = message.Chat.Id;

        if (string.IsNullOrEmpty(msgText)) {
            await SendDefaultMessage(botClient, chatId);
            return;
        }

        // парсинг данных
        var splitedText = msgText.Split(' ');

        if (splitedText.Length != 2) {
            await SendDefaultMessage(botClient, chatId);
            return;
        }

        var groupAltIdToAddStr = splitedText.First();
        if (int.TryParse(groupAltIdToAddStr, out int groupAltIdToAdd) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{groupAltIdToAddStr} не похож на идентификатор группы");
            return;
        }

        var userTgIdStr = splitedText.Last();
        if (long.TryParse(userTgIdStr, out long userTgId) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{userTgIdStr} не похож на идентификатор пользователя Telegram");
            return;
        }

        // поиск данных в бд
        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = groupAltIdToAdd });
        if (group == null) {
            await botClient.SendTextMessageAsync(chatId, $"Группа с идентификатором {groupAltIdToAdd} не найдена");
            return;
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = userTgId });
        if (user == null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь с идентификатором {userTgId} не найден");
            return;
        }

        if (group.UserGroups.Any(e => e.UserUID.Equals(user.UID))) {
            // уже имеется
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {user.Name} уже состоит в группе {group.Name}");
            return;
        }

        // добавление
        var userToGroup = await _mediator.Send(new CreateUserGroupCommand() {
            GroupUID = group.UID,
            UserUID = user.UID
        });

        await botClient.SendTextMessageAsync(chatId, $"Пользователь {user.Name} добавлен в группу {group.Name}");
    }

    private Task SendDefaultMessage(ITelegramBotClient botClient, long chatId) {
        return botClient.SendTextMessageAsync(chatId,
            "Синтаксис для добавления пользователя в группу: /addusertogroup [id группы] [tg id пользователя]");
    }
}