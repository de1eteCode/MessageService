using MessageService.Datas;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
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

        var user = await context.Users.FirstOrDefaultAsync(e => e.Id == message.From!.Id.ToString());
        if (user == null) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Странно, я не нашел тебя в своей базе данных");
            return;
        }

        var groups = context.Groups.Where(e => e.Users!.Any(s => s.Id == user.Id));
        var count = await groups.CountAsync();

        if (count < 1) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ты не состоишь не в одной группе");
            return;
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вот что я знаю о группах, в которых ты состоишь, их всего {count}. {(count > 5 ? "Готовтесь к спаму с:" : "")}");
        await groups.ForEachAsync(group => {
            botClient.SendTextMessageAsync(message.Chat.Id, String.Format("{0} - {1}", group.GroupId, group.Title));
        });
    }
}