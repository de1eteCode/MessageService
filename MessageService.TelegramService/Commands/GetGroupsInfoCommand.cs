using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record GetGroupsInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getgroupsinfo", "Получение информации о всех группах, в которых состоишь");
}

internal class GetGroupsInfoCommandHandler : TelegramRequestHandler<GetGroupsInfoCommand> {
    private readonly IMediator _mediator;

    public GetGroupsInfoCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override Task<Unit> Handle(GetGroupsInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class GetGroupsInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetGroupsInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetGroupsInfoCommand request) {
        throw new NotImplementedException();
    }
}