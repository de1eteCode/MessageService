using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record GetMyIdCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getmyid", "Получение идентификатора вашей учетной записи Telegram");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long SenderUserId { get; set; }
}

internal class GetMyIdCommandHandler : TelegramRequestHandler<GetMyIdCommand> {

    public GetMyIdCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override async Task<Unit> Handle(GetMyIdCommand request, BotClient botClient, CancellationToken cancellationToken) {
        await botClient.SendMessageAsync(request.PrivateChatId, $"Ваш идентификатор: " + request.PrivateChatId, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}

internal class GetMyIdCommandParamsBuilder : ITelegramRequestParamsBuilder<GetMyIdCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetMyIdCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;
        request.SenderUserId = update.Message.From!.Id;
    }
}