using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Удаление чата из группы
/// </summary>
[UserRole("Системный администратор")]
internal class RemoveChatFromGroupCommand : BotCommandAction {
    private readonly IDataContext _context;

    public RemoveChatFromGroupCommand(IDataContext context) : base("removechatfromgroup", "Удаление чата из группы") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage();
            return;
        }

        var splited = msg.Split(" ", 2);
        var chatIdStrToRemove = splited.First();
        var groupIdStrToRemove = splited.Last();

        // парсинг идентификатора группы
        if (int.TryParse(groupIdStrToRemove, out int groupIdToRemove) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToRemove} не похож на идентификатор группы");
            return;
        }

        // парсинг идентификатора чата
        if (long.TryParse(chatIdStrToRemove, out long chatIdToRemove) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToRemove} не похож на идентификатор чата");
            return;
        }

        // поиск чата по идентификатору
        var chat = await _context.Chats.FirstOrDefaultAsync(e => e.TelegramChatId == chatIdToRemove);
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToRemove}");
            return;
        }

        // поиск группы по идентификатору
        var group = await _context.Groups.FirstOrDefaultAsync(e => e.AlternativeId == groupIdToRemove);
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToRemove}");
            return;
        }

        // проверка пользователя на наличие в группе
        var user = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId == message.From!.Id);
        if (user == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Странно, я не нашел твою учетку в своей базе данных");
            return;
        }

        if (group.UserGroups!.Any(e => e.UserUID == user.UID) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Ты не можешь удалять чаты из группы, в которой не состоишь");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await _context.ChatGroups.FirstOrDefaultAsync(e => e.Chat.TelegramChatId!.Equals(chat.TelegramChatId) && e.GroupUID == group.UID && e.IsDeleted == false);
        if (chatToGroup == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" не состоит в группе \"{group.Name}\"");
            return;
        }

        chatToGroup.IsDeleted = true;
        // пометка "удален"
        _context.Entry(chatToGroup).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" успешно удален из группы \"{group.Name}\"");

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис удаления чата из группы /removechatfromgroup [id чата] [id группы]\n" +
                "Узнать доступные группы: /getgroupsinfo\n" +
                "Узнать чаты в группе: /getchatinfo [id группы]");
        }
    }
}