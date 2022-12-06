using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

[TelegramUserRole("Системный администратор")]
internal record LeaveChatByIdCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("leavechatbyid", "Команда для выхода из чата ботом");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор чата, из которого необходимо выйти
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
            var chatInfo = await botClient.GetChatAsync((long)request.LeaveChatId, cancellationToken);

            var iAmIsMember = await botClient.GetChatMemberAsync((long)request.LeaveChatId, botUserInfo.Id, cancellationToken) != null; // Генерирует ошибку с ErrCode = 401 | 403

            if (iAmIsMember) {
                await botClient.LeaveChatAsync((long)request.LeaveChatId, cancellationToken);
                await botClient.SendMessageAsync(request.PrivateChatId, $"Я вышел из чата {chatInfo.Title} ({chatInfo.Id})");
            }
        }
        catch (AggregateException ex) when (ex.InnerException is BotRequestException) {
            await HandleRequestException((ex.InnerException as BotRequestException)!, request.PrivateChatId, cancellationToken);
        }
        catch (BotRequestException ex) {
            await HandleRequestException(ex, request.PrivateChatId, cancellationToken);
        }

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис выхода из чата: /leavechatbyid [tg chat id]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task HandleRequestException(BotRequestException ex, long privateChatId, CancellationToken cancellationToken) {
        var errorAndMessages = new Dictionary<int, string>() {
            { 401, "Не нашел чат" },
            { 403, "Бот не является участником этой группы" },
        };

        var msgToSend = "В ходе выполнения операции произошла ошибка: ";

        if (errorAndMessages.TryGetValue(ex.ErrorCode, out var msg)) {
            msgToSend += msg;
        }
        else {
            msgToSend += ex.Message;
        }

        await _botClient.SendMessageAsync(privateChatId, msgToSend, cancellationToken: cancellationToken);
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