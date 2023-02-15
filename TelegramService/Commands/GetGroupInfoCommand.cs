using Application.Groups.Queries;
using MediatR;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Получение информации о конкретной внутренней роли
/// </summary>
internal class GetGroupInfoCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public GetGroupInfoCommand(IMediator mediator) : base("getgroupinfo", "Получение информации о конкретной группе") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMsg();
            return;
        }

        var ch = message.Chat;

        if (int.TryParse(msg, out int idGroup) == false) {
            await botClient.SendTextMessageAsync(chatId, $"\"{msg}\" не похож на идентификатор группы", cancellationToken: cancellationToken);
            return;
        }
        var findedGroup = await _mediator.Send(new GetGroupCommand() { AlternativeId = idGroup }, cancellationToken);
        if (findedGroup == null) {
            await botClient.SendTextMessageAsync(chatId, $"Я не нашел группу с идентификатором {idGroup}", cancellationToken: cancellationToken);
            return;
        }

        var senderIsJoined = findedGroup!.UserGroups.Any(e => e.User.TelegramId.Equals(message.From!.Id));
        if (senderIsJoined == false) {
            await botClient.SendTextMessageAsync(chatId, $"Я нашел группу с идентификатором {idGroup}, но ты в ней не состоишь, к сожалению", cancellationToken: cancellationToken);
            return;
        }

        await botClient.SendTextMessageAsync(chatId, $"Вот что я знаю о группе {findedGroup.Name} ({findedGroup.AlternativeId})");

        var nameUsersGroup = findedGroup.UserGroups!.Select(e => e.User.Name).ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"Пользователи чата (количество {nameUsersGroup.Count}):");
        nameUsersGroup.ForEach(name => sb.AppendLine("- " + name));
        await botClient.SendTextMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
        sb.Clear();

        var nameChatsGroup = findedGroup.ChatGroups!.Select(e => e.Chat.Name).ToList();
        sb.AppendLine($"Включенные чаты в группу (количество {nameChatsGroup.Count}):");
        nameChatsGroup.ForEach(name => sb.AppendLine("- " + name));
        await botClient.SendTextMessageAsync(chatId, sb.ToString());
        sb.Clear();

        return;

        Task SendDefaultMsg() {
            return botClient.SendTextMessageAsync(chatId,
                "Синтаксис получения информации о группе: /getgroupinfo [id группы]\n" +
                "Узнать доступные группы: /getgroupsinfo");
        }
    }
}