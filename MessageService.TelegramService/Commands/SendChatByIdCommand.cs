using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

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

    public override Task<Unit> Handle(SendChatByIdCommand request, BotClient botClient, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "",
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