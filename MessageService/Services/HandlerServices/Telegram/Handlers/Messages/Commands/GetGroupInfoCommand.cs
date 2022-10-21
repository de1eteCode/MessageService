using System.Text;
using MessageService.Datas;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Получение информации о конкретной внутренней роли
/// </summary>
[TelegramUserRole("Системный администратор")]
public class GetGroupInfoCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public GetGroupInfoCommand(IDatabaseService<DataContext> dbService) : base("getgroupinfo", "Получение информации о конкретной группе") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMsg();
            return;
        }

        if (int.TryParse(msg, out int idGroup)) {
            var context = _dbService.GetDBContext();
            var findedGroup = await context.Groups!.FirstOrDefaultAsync(e => e.GroupId == idGroup && e.Users!.Any(s => s.Id!.Equals(message.From!.Id.ToString())));

            if (findedGroup != null) {
                await botClient.SendTextMessageAsync(chatId, $"Вот что я знаю о группе {findedGroup.Title} ({findedGroup.GroupId})");

                var nameUsersGroup = findedGroup.Users!.Select(e => e.Name).ToList();
                var sb = new StringBuilder();
                sb.AppendLine($"Пользователи чата (количество {nameUsersGroup.Count}):");
                nameUsersGroup.ForEach(name => sb.AppendLine("- " + name));
                await botClient.SendTextMessageAsync(chatId, sb.ToString());
                sb.Clear();

                var nameChatsGroup = await context.ChatGroups.Where(e => e.GroupId == findedGroup.GroupId && e.IsDeleted == false).Select(e => e.Chat!.Name).ToListAsync();
                sb.AppendLine($"Включенные чаты в группу (количество {nameChatsGroup.Count}):");
                nameChatsGroup.ForEach(name => sb.AppendLine("- " + name));
                await botClient.SendTextMessageAsync(chatId, sb.ToString());
                sb.Clear();
            }
            else {
                await botClient.SendTextMessageAsync(chatId, $"Я не нашел для тебя группу с идентификатором {idGroup}");
            }
        }
        else {
            await botClient.SendTextMessageAsync(chatId, $"\"{msg}\" не похож на идентификатор группы");
        }

        return;

        Task SendDefaultMsg() {
            return botClient.SendTextMessageAsync(chatId,
                "Синтаксис получения информации о группе: /getgroupinfo [id группы]\n" +
                "Узнать доступные группы: /getgroupsinfo");
        }
    }
}