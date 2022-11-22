using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record GetChatsInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getchatsinfo", "Получение информации о всех чатах, которые есть в БД");
}

internal class GetChatsInfoCommandHandler : TelegramRequestHandler<GetChatsInfoCommand> {

    public GetChatsInfoCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override Task<Unit> Handle(GetChatsInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class GetChatsInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetChatsInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetChatsInfoCommand request) {
        throw new NotImplementedException();
    }
}