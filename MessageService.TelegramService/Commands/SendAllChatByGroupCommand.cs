using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record SendAllChatByGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendallchatbygroup", "Отправка сообщения во все чаты, которые имееются в группе");
}

internal class SendAllChatByGroupCommandHandler : TelegramRequestHandler<SendAllChatByGroupCommand> {
    private readonly IMediator _mediator;

    public SendAllChatByGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override Task<Unit> Handle(SendAllChatByGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class SendAllChatByGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<SendAllChatByGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendAllChatByGroupCommand request) {
        throw new NotImplementedException();
    }
}