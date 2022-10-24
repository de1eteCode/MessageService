using System.Text;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using DataLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
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
            var findedGroup = await context.Groups!.FirstOrDefaultAsync(e => e.AlternativeId == idGroup && e.UserGroups!.Any(s => s.User!.TelegramId.Equals(message.From!.Id)));

            if (findedGroup != null) {
                await botClient.SendTextMessageAsync(chatId, $"Вот что я знаю о группе {findedGroup.Name} ({findedGroup.AlternativeId})");

                var nameUsersGroup = findedGroup.UserGroups!.Select(e => e.User.Name).ToList();
                var sb = new StringBuilder();
                sb.AppendLine($"Пользователи чата (количество {nameUsersGroup.Count}):");
                nameUsersGroup.ForEach(name => sb.AppendLine("- " + name));
                await botClient.SendTextMessageAsync(chatId, sb.ToString());
                sb.Clear();

                var nameChatsGroup = await context.ChatGroups.Where(e => e.GroupUID == findedGroup.UID && e.IsDeleted == false).Select(e => e.Chat!.Name).ToListAsync();
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