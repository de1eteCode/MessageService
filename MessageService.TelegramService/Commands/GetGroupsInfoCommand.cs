using Application.Groups.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

[TelegramUserRole("Системный администратор")]
internal record GetGroupsInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getgroupsinfo", "Получение информации о всех группах, в которых состоишь");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор пользователя ТГ
    /// </summary>
    public long SenderUserId { get; set; }
}

internal class GetGroupsInfoCommandHandler : TelegramRequestHandler<GetGroupsInfoCommand> {
    private readonly IMediator _mediator;

    public GetGroupsInfoCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(GetGroupsInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        var groups = await _mediator.Send(new GetGroupsCommand() {
            Predicate = group => group.UserGroups.Any(e => e.User.TelegramId.Equals(request.SenderUserId))
        });

        if (groups.Any()) {
            var sb = new StringBuilder();

            foreach (var group in groups.OrderBy(e => e.AlternativeId)) {
                sb.AppendLine(String.Format("{0} - {1}", group.AlternativeId, group.Name));
            };

            await botClient.SendMessageAndSplitIfOverfullAsync(request.PrivateChatId, sb.ToString(), cancellationToken: cancellationToken);
        }
        else {
            await botClient.SendMessageAsync(request.PrivateChatId, "Вы не состоите ни в одной группе", cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}

internal class GetGroupsInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetGroupsInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetGroupsInfoCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;
        request.SenderUserId = update.Message!.From!.Id;
    }
}