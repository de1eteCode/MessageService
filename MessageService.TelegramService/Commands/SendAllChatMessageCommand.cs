using Application.Chats.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;
// TODO:
internal record SendAllChatMessageCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendallchat", "Отправка сообщения во все чаты");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Текст сообщения для отправки
    /// </summary>
    public string? Message { get; set; }
}

internal class SendAllChatMessageCommandHandler : TelegramRequestHandler<SendAllChatMessageCommand> {
    private readonly IMediator _mediator;

    public SendAllChatMessageCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(SendAllChatMessageCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.Message)) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var chats = await _mediator.Send(new GetChatsCommand() { Predicate = chat => chat.IsJoined }, cancellationToken);

        throw new NotImplementedException();
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис для отправки сообщения во все чаты: /sendallchat [текст сообщения]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class SendAllChatMessageCommandParamsBuilder : ITelegramRequestParamsBuilder<SendAllChatMessageCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendAllChatMessageCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 1) {
            return;
        }

        request.Message = string.Join(" ", args);
    }
}