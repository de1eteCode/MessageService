using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Добавление пользователя в группу
/// </summary>
[UserRole("Системный администратор")]
internal class AddUserToGroupCommand : BotCommandAction {
    private readonly IDataContext _context;

    public AddUserToGroupCommand(IDataContext context) : base("addusertogroup", "Добавление пользователя в группу") {
        _context = context;
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

        // поиск данных в бд
        var group = await _context.Groups.SingleOrDefaultAsync(e => e.AlternativeId.Equals(groupAltIdToAdd));
        if (group == null) {
            await botClient.SendTextMessageAsync(chatId, $"Группа с идентификатором {groupAltIdToAdd} не найдена");
            return;
        }

        var user = await _context.Users.SingleOrDefaultAsync(e => e.TelegramId.Equals(userTgId));
        if (user == null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь с идентификатором {userTgId} не найден");
            return;
        }

        var userToGroup = await _context.UserGroups.SingleOrDefaultAsync(e => e.UserUID.Equals(user.UID) && e.GroupUID.Equals(group.UID));
        if (userToGroup == null) {
            // добавление
            userToGroup = new Domain.Models.UserGroup() {
                Group = group,
                User = user
            };

            _context.Entry(userToGroup).State = EntityState.Added;
            await _context.SaveChangesAsync();

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