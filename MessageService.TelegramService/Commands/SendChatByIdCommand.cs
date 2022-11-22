using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record SendChatByIdCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendchatbyid", "Отправка сообщения во все чаты");
}

internal class SendChatByIdCommandHandler : TelegramRequestHandler<SendChatByIdCommand> {

    public SendChatByIdCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override Task<Unit> Handle(SendChatByIdCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}

internal class SendChatByIdCommandParamsBuilder : ITelegramRequestParamsBuilder<SendChatByIdCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendChatByIdCommand request) {
        throw new NotImplementedException();
    }
}