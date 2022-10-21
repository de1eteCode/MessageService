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

        var context = _dbService.GetDBContext();
        var addedGroupUser = await context.Users.FirstOrDefaultAsync(e => e.Id!.Equals(message.From!.Id.ToString()));

        if (addedGroupUser == null) {
            await botClient.SendTextMessageAsync(chatId, "Странно, я не нашел твоей учетной записи у себя в базе");
            return;
        }

        var newGroup = new Group() {
            Title = msg,
            Users = new List<Datas.Models.User>() {
                addedGroupUser
            }
        };

        context.Entry(newGroup).State = EntityState.Added;
        await context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(chatId, $"Группа \"{newGroup.Title}\" успешно добавлена под идентификатором: {newGroup.GroupId}");
    }
}