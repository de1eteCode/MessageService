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

        var splited = msg.Split(" ");
        if (splited.Length != 2) {
            await SendDefaultMessage();
            return;
        }

        var chatIdToAdd = splited.First();
        var groupIdStrToAdd = splited.Last();
        int groupIdToAdd = -1;

        // парсинг идентификатора группы
        if (int.TryParse(groupIdStrToAdd, out groupIdToAdd) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdStrToAdd} не похож на идентификатор группы");
            return;
        }

        var context = _dbService.GetDBContext();

        // поиск чата по идентификатору
        var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId == chatIdToAdd);
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToAdd}");
            return;
        }

        // поиск группы по идентификатору
        var group = await context.Groups.FirstOrDefaultAsync(e => e.GroupId == groupIdToAdd);
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToAdd}");
            return;
        }

        // проверка пользователя на наличие в группе
        var user = await context.Users.FirstOrDefaultAsync(e => e.Id == message.From!.Id.ToString());
        if (user == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Странно, я не нашел твою учетку в своей базе данных");
            return;
        }

        if (group.Users!.Any(e => e.Id == user.Id) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Ты не можешь добавлять чаты в группу, в которой не состоишь");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await context.ChatGroups.FirstOrDefaultAsync(e => e.ChatId!.Equals(chat.ChatId) && e.GroupId == group.GroupId);
        if (chatToGroup != null) {
            // если имеется
            if (chatToGroup.IsDeleted) {
                chatToGroup.IsDeleted = false;
                context.Entry(chatToGroup).State = EntityState.Modified;
            }
            else {
                await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" уже состоит в группе \"{group.Title}\"");
                return;
            }
        }
        else {
            chatToGroup = new ChatGroup() {
                Chat = chat,
                ChatId = chat.ChatId,
                Group = group,
                GroupId = group.GroupId,
                IsDeleted = false
            };

            context.Entry(chatToGroup).State = EntityState.Added;
        }

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