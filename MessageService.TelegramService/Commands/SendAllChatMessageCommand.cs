using Application.Chats.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

[TelegramUserRole("Системный администратор")]
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

        var countSended = 0;

        var tasksToSend = chats.Select(chat => Task.Run(async () => {
            try {
                var res = await botClient.SendMessageAndSplitIfOverfullAsync(chat.TelegramChatId, request.Message, cancellationToken: cancellationToken);

                if (res != null) {
                    Interlocked.Increment(ref countSended);
                }
            }
            catch (Exception) {
                // Todo: Обработка ошибки, если бота кикнули из чата, когда был оффлайн
            }
        }));

        await Task.WhenAll(tasksToSend);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Сообщение было отправлено в {countSended} чат{GetRu(countSended)}", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private string GetRu(int num) {
        if (num > 10) {
            num = num % 10;
        }

        if (num == 1) {
            return "";
        }

        if (num > 1 && num < 5) {
            return "а";
        }

        return "ов";
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