using System.Text;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Получение информации о конкретной внутренней роли
/// </summary>
[UserRole("Системный администратор")]
internal class GetGroupInfoCommand : BotCommandAction {
    private readonly IDataContext _context;

    public GetGroupInfoCommand(IDataContext context) : base("getgroupinfo", "Получение информации о конкретной группе") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMsg();
            return;
        }

        if (int.TryParse(msg, out int idGroup)) {
            var findedGroup = await _context.Groups!.FirstOrDefaultAsync(e => e.AlternativeId == idGroup && e.UserGroups!.Any(s => s.User!.TelegramId.Equals(message.From!.Id)));

            if (findedGroup != null) {
                await botClient.SendTextMessageAsync(chatId, $"Вот что я знаю о группе {findedGroup.Name} ({findedGroup.AlternativeId})");

                var nameUsersGroup = findedGroup.UserGroups!.Select(e => e.User.Name).ToList();
                var sb = new StringBuilder();
                sb.AppendLine($"Пользователи чата (количество {nameUsersGroup.Count}):");
                nameUsersGroup.ForEach(name => sb.AppendLine("- " + name));
                await botClient.SendTextMessageAsync(chatId, sb.ToString());
                sb.Clear();

                var nameChatsGroup = await _context.ChatGroups.Where(e => e.GroupUID == findedGroup.UID && e.IsDeleted == false).Select(e => e.Chat!.Name).ToListAsync();
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