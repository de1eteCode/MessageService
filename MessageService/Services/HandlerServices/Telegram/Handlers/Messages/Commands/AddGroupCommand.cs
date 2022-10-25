﻿using MessageService.Services.HandlerServices.Telegram.Attributes;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using DataLibrary.Helpers;
using DataLibrary.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Добавление новой группы
/// </summary>
[TelegramUserRole("Системный администратор")]
public class AddGroupCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public AddGroupCommand(IDatabaseService<DataContext> dbService) : base("addgroup", "Добавление новой группы") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await botClient.SendTextMessageAsync(chatId, "Синтаксис добавления новой группы: /addgroup [наименование группы]");
            return;
        }

        var context = _dbService.GetDBContext();
        var addedGroupUser = await context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(message.From!.Id));

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

        context.Entry(newGroup).State = EntityState.Added;
        context.Entry(userGroup).State = EntityState.Added;
        await context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(chatId, $"Группа \"{newGroup.Name}\" успешно добавлена под идентификатором: {newGroup.AlternativeId}");
    }
}