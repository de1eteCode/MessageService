using Application.Users.Queries;
using MediatR;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Extensions;

namespace TelegramService.Commands;

/// <summary>
/// Получение информации о всех внутренних группах
/// </summary>
internal class GetGroupsInfoCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public GetGroupsInfoCommand(IMediator mediator) : base("getgroupsinfo", "Получение информации о всех группах, в которых состоишь") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var user = await _mediator.Send(new GetUserCommand() { TelegramId = message.From!.Id }, cancellationToken);
        if (user == null) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Странно, я не нашел тебя в своей базе данных", cancellationToken: cancellationToken);
            return;
        }

        var count = user.UserGroups.Count();

        if (count < 1) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ты не состоишь не в одной группе", cancellationToken: cancellationToken);
            return;
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вот что я знаю о группах, в которых ты состоишь, их всего {count}.", cancellationToken: cancellationToken);

        var sb = new StringBuilder();

        foreach (var userGroup in user.UserGroups) {
            var group = userGroup.Group;
            sb.AppendLine(String.Format("{0} - {1}", group.AlternativeId, group.Name));
        };

        await botClient.SendTextMessageAndSplitIfOverfullAsync(message.Chat.Id, sb.ToString(), cancellationToken: cancellationToken);
    }
}