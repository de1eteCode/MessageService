using DataLibrary;
using DataLibrary.Helpers;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Добавление пользователя в группу
/// </summary>
[TelegramUserRole("Системный администратор")]
public class AddUserToGroupCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public AddUserToGroupCommand(IDatabaseService<DataContext> dbService) : base("addusertogroup", "Добавление пользователя в группу") {
        _dbService = dbService;
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
        if (long.TryParse(groupAltIdToAddStr, out long groupAltIdToAdd) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{groupAltIdToAddStr} не похож на идентификатор группы");
            return;
        }

        var userTgIdStr = splitedText.Last();
        if (long.TryParse(userTgIdStr, out long userTgId) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{userTgIdStr} не похож на идентификатор пользователя Telegram");
            return;
        }

        var context = _dbService.GetDBContext();


        var ownerUser = await context.Users.SingleOrDefaultAsync(e => e.TelegramId.Equals(message.From!.Id));
        if (ownerUser == null) {
            await botClient.SendTextMessageAsync(chatId, "Странно, я не нашел тебя в своей базе данных");
            return;
        }

        // поиск данных в бд
        var group = ownerUser.UserGroups.FirstOrDefault(e => e.Group.AlternativeId.Equals(groupAltIdToAdd));
        if (group == null) {
            await botClient.SendTextMessageAsync(chatId, $"Группа с идентификатором {groupAltIdToAdd} не найдена");
            return;
        }

        var user = await context.Users.SingleOrDefaultAsync(e => e.TelegramId.Equals(userTgId));
        if (user == null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь с идентификатором {userTgId} не найден");
            return;
        }

        var userToGroup = await context.UserGroups.SingleOrDefaultAsync(e => e.UserUID.Equals(user.UID) && e.GroupUID.Equals(group.UID));
        if (userToGroup == null) {
            // добавление
            userToGroup = new DataLibrary.Models.UserGroup() {
                Group = group,
                User = user
            };

            context.Entry(userToGroup).State = EntityState.Added;
            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(chatId, $"Пользователь {user.Name} добавлен в группу {group.Name}");
        }
        else {
            // уже имеется
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {user.Name} уже состоит в группе {group.Name}");
        }
    }

    private Task SendDefaultMessage(ITelegramBotClient botClient, long chatId) {
        return botClient.SendTextMessageAsync(chatId,
            "Синтаксис для добавления пользователя в группу: /addusertogroup [id группы] [tg id пользователя]");
    }
}