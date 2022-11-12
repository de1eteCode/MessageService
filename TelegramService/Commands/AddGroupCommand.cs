using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Добавление новой группы
/// </summary>
[UserRole("Системный администратор")]
internal class AddGroupCommand : BotCommandAction {
    private readonly IDataContext _context;

    public AddGroupCommand(IDataContext context) : base("addgroup", "Добавление новой группы") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await botClient.SendTextMessageAsync(chatId, "Синтаксис добавления новой группы: /addgroup [наименование группы]");
            return;
        }

        var addedGroupUser = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(message.From!.Id));

        if (addedGroupUser == null) {
            await botClient.SendTextMessageAsync(chatId, "Странно, я не нашел твоей учетной записи у себя в базе");
            return;
        }

        var newGroup = new Group() {
            Name = msg
        };

        var userGroup = new UserGroup() {
            Group = newGroup,
            User = addedGroupUser,
        };

        _context.Entry(newGroup).State = EntityState.Added;
        _context.Entry(userGroup).State = EntityState.Added;
        await _context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(chatId, $"Группа \"{newGroup.Name}\" успешно добавлена под идентификатором: {newGroup.AlternativeId}");
    }
}