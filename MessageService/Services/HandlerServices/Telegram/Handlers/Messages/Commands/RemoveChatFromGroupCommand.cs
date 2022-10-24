using MessageService.Services.HandlerServices.Telegram.Attributes;
using RepositoryLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Удаление чата из группы
/// </summary>
[TelegramUserRole("Системный администратор")]
public class RemoveChatFromGroupCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public RemoveChatFromGroupCommand(IDatabaseService<DataContext> dbService) : base("removechatfromgroup", "Удаление чата из группы") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage();
            return;
        }

        var splited = msg.Split(" ", 2);
        var chatIdToRemove = splited.First();
        var groupIdStrToRemove = splited.Last();
        int groupIdToRemove = -1;

        // парсинг идентификатора группы
        if (int.TryParse(groupIdStrToRemove, out groupIdToRemove) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToRemove} не похож на идентификатор группы");
            return;
        }

        var context = _dbService.GetDBContext();

        // поиск чата по идентификатору
        var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId == chatIdToRemove);
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToRemove}");
            return;
        }

        // поиск группы по идентификатору
        var group = await context.Groups.FirstOrDefaultAsync(e => e.GroupId == groupIdToRemove);
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToRemove}");
            return;
        }

        // проверка пользователя на наличие в группе
        var user = await context.Users.FirstOrDefaultAsync(e => e.Id == message.From!.Id.ToString());
        if (user == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Странно, я не нашел твою учетку в своей базе данных");
            return;
        }

        if (group.Users!.Any(e => e.Id == user.Id) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Ты не можешь удалять чаты из группы, в которой не состоишь");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await context.ChatGroups.FirstOrDefaultAsync(e => e.ChatId!.Equals(chat.ChatId) && e.GroupId == group.GroupId && e.IsDeleted == false);
        if (chatToGroup == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" не состоит в группе \"{group.Title}\"");
            return;
        }

        chatToGroup.IsDeleted = true;
        // пометка "удален"
        context.Entry(chatToGroup).State = EntityState.Modified;
        await context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" успешно удален из группы \"{group.Title}\"");

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис удаления чата из группы /removechatfromgroup [id чата] [id группы]\n" +
                "Узнать доступные группы: /getgroupsinfo\n" +
                "Узнать чаты в группе: /getchatinfo [id группы]");
        }
    }
}