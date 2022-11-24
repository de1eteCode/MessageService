using Application.Groups.Queries;
using Domain.Models;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record GetGroupInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getgroupinfo", "Получение информации о конкретной группе");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор пользователя ТГ
    /// </summary>
    public long SenderUserId { get; set; }

    /// <summary>
    /// Альтернативный идентификатор <see cref="Group"/>
    /// </summary>
    public int? GroupAlternativeId { get; set; }
}

internal class GetGroupInfoCommandHandler : TelegramRequestHandler<GetGroupInfoCommand> {
    private readonly IMediator _mediator;

    public GetGroupInfoCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(GetGroupInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.GroupAlternativeId == null) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = (int)request.GroupAlternativeId });

        if (group == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Я не нашел группу с идентификатором {request.GroupAlternativeId}", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var userIsJoined = group.UserGroups.Any(e => e.User.TelegramId.Equals(request.SenderUserId));

        if (userIsJoined == false) {
            await botClient.SendMessageAsync(request.PrivateChatId, "Вы не можете получить информацию о группе, в которой не состоите", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        await botClient.SendMessageAsync(request.PrivateChatId, $"Вот что я знаю о группе {group.Name} ({group.AlternativeId})");

        var nameUsersGroup = group.UserGroups!.Select(e => e.User.Name).ToList();

        var sb = new StringBuilder();

        sb.AppendLine($"Пользователи чата (количество {nameUsersGroup.Count}):");

        nameUsersGroup.ForEach(name => sb.AppendLine("- " + name));

        await botClient.SendMessageAsync(request.PrivateChatId, sb.ToString(), cancellationToken: cancellationToken);

        sb.Clear();

        var nameChatsGroup = group.ChatGroups!.Select(e => e.Chat.Name).ToList();

        sb.AppendLine($"Включенные чаты в группу (количество {nameChatsGroup.Count}):");

        nameChatsGroup.ForEach(name => sb.AppendLine("- " + name));

        await botClient.SendMessageAsync(request.PrivateChatId, sb.ToString());

        sb.Clear();

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис получения информации о группе: /getgroupinfo [id группы]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class GetGroupInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetGroupInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetGroupInfoCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;
        request.SenderUserId = update.Message!.From!.Id;

        if (args.Count() < 1) {
            return;
        }

        if (int.TryParse(args.ElementAt(0), out var groupAltId)) {
            request.GroupAlternativeId = groupAltId;
        }
    }
}