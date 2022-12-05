using Application.Chats.Queries;
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
internal record SendChatByIdCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendchatbyid", "Отправка сообщения во все чаты");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор чата, в которое необходимо отправить сообщение
    /// </summary>
    public long? RecieveChatId { get; set; }

    /// <summary>
    /// Текст сообщения для отправки
    /// </summary>
    public string? Message { get; set; }
}

internal class SendChatByIdCommandHandler : TelegramRequestHandler<SendChatByIdCommand> {
    private readonly IMediator _mediator;

    public SendChatByIdCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(SendChatByIdCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.RecieveChatId == null || string.IsNullOrEmpty(request.Message)) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var chatToSend = await _mediator.Send(new GetChatCommand() { TelegramChatId = (long)request.RecieveChatId });

        if (chatToSend == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Не нашел группу с идентификатором {request.RecieveChatId}", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (chatToSend.IsJoined == false) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Я не состою в {chatToSend.Name} ({chatToSend.TelegramChatId})", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var resToSend = await botClient.SendMessageAsync(chatToSend.TelegramChatId, request.Message, cancellationToken: cancellationToken);

        if (resToSend != null) {
            var titleChat = string.Empty;

            if (string.IsNullOrEmpty(resToSend.Chat.Title)) {
                titleChat = string.Join(" ", resToSend.Chat.FirstName, resToSend.Chat.LastName);
            }
            else {
                titleChat = resToSend.Chat.Title;
            }

            await botClient.SendMessageAsync(request.PrivateChatId, $"Сообщение отправлено в \"{titleChat}\"", cancellationToken: cancellationToken);
        }
        else {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Сообщение не было доставлено ", cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис для отправки сообщения в чат: /sendchatbyid [chat id tg] [текст сообщения]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class SendChatByIdCommandParamsBuilder : ITelegramRequestParamsBuilder<SendChatByIdCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendChatByIdCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 2) {
            return;
        }

        if (long.TryParse(args.ElementAt(0), out var chatId)) {
            request.RecieveChatId = chatId;
        }

        request.Message = string.Join(" ", args.Skip(1));
    }
}