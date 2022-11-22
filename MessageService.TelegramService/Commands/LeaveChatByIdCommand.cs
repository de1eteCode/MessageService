using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record LeaveChatByIdCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("leavechatbyid", "Команда для выхода из чата ботом");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long? LeaveChatId { get; set; }
}

internal class LeaveChatByIdCommandHandler : TelegramRequestHandler<LeaveChatByIdCommand> {

    public LeaveChatByIdCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override async Task<Unit> Handle(LeaveChatByIdCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.LeaveChatId == null) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var botUserInfo = await botClient.GetMeAsync(cancellationToken);

        try {
        }
        catch (BotRequestException ex) {
        }

        throw new NotImplementedException();
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис выхода из чата: /leavechatbyid [tg chat id]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class LeaveChatByIdCommandParamsBuilder : ITelegramRequestParamsBuilder<LeaveChatByIdCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref LeaveChatByIdCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 1) {
            return;
        }

        if (long.TryParse(args.ElementAt(0), out var leaveChatId)) {
            request.LeaveChatId = leaveChatId;
        }
    }
}