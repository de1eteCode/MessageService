using Application.ChatGroups.Commands.UpdateChatGroup;
using Application.ChatGroups.Queries.GetChatGroup;
using Application.Chats.Queries.GetChat;
using Application.Groups.Queries.GetGroup;
using Application.Users.Queries.GetUser;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Удаление чата из группы
/// </summary>
[UserRole("Системный администратор")]
internal class RemoveChatFromGroupCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public RemoveChatFromGroupCommand(IMediator mediator) : base("removechatfromgroup", "Удаление чата из группы") {
        _mediator = mediator;
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
        var chat = await _mediator.Send(new GetChatCommand() { TelegramChatId = chatIdToRemove });
        if (chat == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Я не знаю о чате с идентификатором {chatIdToRemove}");
            return;
        }

        // поиск группы по идентификатору
        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = groupIdToRemove });
        if (group == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"У меня нет группы с идентификатором {groupIdToRemove}");
            return;
        }

        // проверка пользователя на наличие в группе
        var user = await _mediator.Send(new GetUserCommand() { TelegramId = message.From!.Id });
        if (user == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Странно, я не нашел твою учетку в своей базе данных");
            return;
        }

        if (group.UserGroups!.Any(e => e.UserUID == user.UID) == false) {
            await botClient.SendTextMessageAsync(privateChatId, $"Ты не можешь удалять чаты из группы, в которой не состоишь");
            return;
        }

        // проверка наличия чата в группе
        var chatToGroup = await _mediator.Send(new GetChatGroupCommand() { ChatUID = chat.UID, GroupUID = group.UID });
        if (chatToGroup == null || chatToGroup.IsDeleted) {
            await botClient.SendTextMessageAsync(privateChatId, $"Чат \"{chat.Name}\" не состоит в группе \"{group.Name}\"");
            return;
        }

        chatToGroup = await _mediator.Send(new UpdateChatGroupCommand() { ChatGroupUID = chatToGroup.UID, IsDeleted = true });

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