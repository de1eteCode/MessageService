using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record GetGroupInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getgroupinfo", "Получение информации о конкретной группе");
}

internal class GetGroupInfoCommandHandler : TelegramRequestHandler<GetGroupInfoCommand> {

    public GetGroupInfoCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override Task<Unit> Handle(GetGroupInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class GetGroupInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetGroupInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetGroupInfoCommand request) {
        throw new NotImplementedException();
    }
}