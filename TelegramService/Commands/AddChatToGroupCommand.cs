using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Команда добавления чата в команду
/// </summary>
[UserRole("Системный администратор")]
internal class AddChatToGroupCommand : BotCommandAction {
    private readonly IDataContext _context;

    public AddChatToGroupCommand(IDataContext context) : base("addchattogroup", "Добавление чата в группу") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage();
            return;
        }

        var splited = msg.Split(" ");
        if (splited.Length != 2) {
            await SendDefaultMessage();
            return;
        }

        var chatIdStrToAdd = splited.First();
        var groupIdStrToAdd = splited.Last();
        int groupIdToAdd = -1;

        // парсинг идентификатора группы
        if (int.TryParse(groupIdStrToAdd, out groupIdToAdd) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToAdd} не похож на идентификатор группы");
            return;
        }

        // парсинг идентификатора чата
        if (long.TryParse(chatIdStrToAdd, out long chatIdToAdd) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {chatIdStrToAdd} не похож на идентификатор чата");
            return;
        }

        // поиск чата по идентификатору
        var chat = await _context.Chats.FirstOrDefaultAsync(e => e.TelegramChatId == chatIdToAdd);
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToAdd}");
            return;
        }

        // поиск группы по идентификатору
        var group = await _context.Groups.FirstOrDefaultAsync(e => e.AlternativeId == groupIdToAdd);
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToAdd}");
            return;
        }

        // проверка пользователя на наличие в группе
        var user = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId == message.From!.Id);
        if (user == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Странно, я не нашел твою учетку в своей базе данных");
            return;
        }

        if (group.UserGroups!.Any(e => e.UserUID == user.UID) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Ты не можешь добавлять чаты в группу, в которой не состоишь");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await _context.ChatGroups.FirstOrDefaultAsync(e => e.ChatUID!.Equals(chat.UID) && e.GroupUID == group.UID);
        if (chatToGroup != null) {
            // если имеется
            if (chatToGroup.IsDeleted) {
                chatToGroup.IsDeleted = false;
                _context.Entry(chatToGroup).State = EntityState.Modified;
            }
            else {
                await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" уже состоит в группе \"{group.Name}\"");
                return;
            }
        }
        else {
            chatToGroup = new ChatGroup() {
                Chat = chat,
                ChatUID = chat.UID,
                Group = group,
                GroupUID = group.UID,
                IsDeleted = false
            };

            _context.Entry(chatToGroup).State = EntityState.Added;
        }

        await _context.SaveChangesAsync();
        await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" успешно добавлен в группу \"{group.Name}\"");

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис добавления чата в группу: /addchattogroup [id чата] [id группы]\n" +
                "Узнать доступные чаты: /getchatsinfo\n" +
                "Узнать доступные группы: /getgroupsinfo");
        }
    }
}