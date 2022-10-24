using System.Text;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using DataLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Получение информации о всех внутренних группах
/// </summary>
[TelegramUserRole("Системный администратор")]
public class GetGroupsInfoCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public GetGroupsInfoCommand(IDatabaseService<DataContext> dbService) : base("getgroupsinfo", "Получение информации о всех группах, в которых состоишь") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var context = _dbService.GetDBContext();

        var user = await context.Users.FirstOrDefaultAsync(e => e.TelegramId == message.From!.Id);
        if (user == null) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Странно, я не нашел тебя в своей базе данных");
            return;
        }

        var groups = context.Groups.Where(e => e.UserGroups!.Any(s => s.UserUID == user.UID));
        var count = await groups.CountAsync();

        if (count < 1) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ты не состоишь не в одной группе");
            return;
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вот что я знаю о группах, в которых ты состоишь, их всего {count}.");
        var sb = new StringBuilder();
        await groups.ForEachAsync(group => {
            sb.AppendLine(String.Format("{0} - {1}", group.AlternativeId, group.Name));
        });
        await botClient.SendTextMessageAndSplitIfOverfullAsync(message.Chat.Id, sb.ToString());
    }
}