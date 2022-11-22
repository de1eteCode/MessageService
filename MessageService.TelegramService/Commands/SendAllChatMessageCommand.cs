using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record SendAllChatMessageCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendallchat", "Отправка сообщения во все чаты");
}

internal class SendAllChatMessageCommandHandler : TelegramRequestHandler<SendAllChatMessageCommand> {
    private readonly IMediator _mediator;

    public SendAllChatMessageCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override Task<Unit> Handle(SendAllChatMessageCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class SendAllChatMessageCommandParamsBuilder : ITelegramRequestParamsBuilder<SendAllChatMessageCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendAllChatMessageCommand request) {
        throw new NotImplementedException();
    }
}