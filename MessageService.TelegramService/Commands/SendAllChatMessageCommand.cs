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

    public SendAllChatMessageCommandHandler(BotClient botClient) : base(botClient) {
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