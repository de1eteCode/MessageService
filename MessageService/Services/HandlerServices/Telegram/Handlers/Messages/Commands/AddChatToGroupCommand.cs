using MessageService.Datas;
using MessageService.Datas.Models;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Команда добавления чата в команду
/// </summary>
[TelegramUserRole("Системный администратор")]
public class AddChatToGroupCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public AddChatToGroupCommand(IDatabaseService<DataContext> dbService) : base("addchattogroup", "Добавление чата в группу") {
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
        var chatIdToAdd = splited.First();
        var groupIdStrToAdd = splited.Last();
        int groupIdToAdd = -1;

        if (int.TryParse(groupIdStrToAdd, out groupIdToAdd) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToAdd} не похож на идентификатор группы");
            return;
        }

        var context = _dbService.GetDBContext();

        var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId == chatIdToAdd);
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToAdd}");
            return;
        }

        var group = await context.Groups.FirstOrDefaultAsync(e => e.GroupId == groupIdToAdd);
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToAdd}");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await context.ChatGroups.FirstOrDefaultAsync(e => e.ChatId!.Equals(chat.ChatId) && e.GroupId == group.GroupId);
        if (chatToGroup != null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" уже состоит в группе \"{group.Title}\"");
            return;
        }

        chatToGroup = new ChatGroup() {
            Chat = chat,
            ChatId = chat.ChatId,
            Group = group,
            GroupId = group.GroupId
        };

        context.Entry(chatToGroup).State = EntityState.Added;
        context.Entry(chat).State = EntityState.Unchanged;
        context.Entry(group).State = EntityState.Unchanged;
        await context.SaveChangesAsync();

        await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" успешно добавлен в группу \"{group.Title}\"");

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис добавления чата в группу: /addchattogroup [id чата] [id группы]\n" +
                "Узнать доступные чаты: /getchatsinfo\n" +
                "Узнать доступные группы: /getgroupsinfo");
        }
    }
}