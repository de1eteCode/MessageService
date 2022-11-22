using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record RemoveChatFromGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("removechatfromgroup", "Удаление чата из группы");
}

internal class RemoveChatFromGroupCommandHandler : TelegramRequestHandler<RemoveChatFromGroupCommand> {
    private readonly IMediator _mediator;

    public RemoveChatFromGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override Task<Unit> Handle(RemoveChatFromGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class RemoveChatFromGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<RemoveChatFromGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref RemoveChatFromGroupCommand request) {
        throw new NotImplementedException();
    }
}