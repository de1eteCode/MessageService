using MessageService.Datas;
using MessageService.Datas.Models;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
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

        var newGroup = new Group() {
            Title = msg
        };

        var context = _dbService.GetDBContext();
        context.Entry(newGroup).State = EntityState.Added;
        await context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(chatId, $"Группа \"{newGroup.Title}\" успешно добавлена под идентификатором: {newGroup.GroupId}");
    }
}