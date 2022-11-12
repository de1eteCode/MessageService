using System.Text;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;
using TelegramService.Extensions;

namespace TelegramService.Commands;

/// <summary>
/// Получение информации о всех внутренних группах
/// </summary>
[UserRole("Системный администратор")]
internal class GetGroupsInfoCommand : BotCommandAction {
    private readonly IDataContext _context;

    public GetGroupsInfoCommand(IDataContext context) : base("getgroupsinfo", "Получение информации о всех группах, в которых состоишь") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var user = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId == message.From!.Id);
        if (user == null) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Странно, я не нашел тебя в своей базе данных");
            return;
        }

        var groups = _context.Groups.Where(e => e.UserGroups!.Any(s => s.UserUID == user.UID));
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