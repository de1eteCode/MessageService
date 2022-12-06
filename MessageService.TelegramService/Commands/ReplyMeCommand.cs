using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;
internal record ReplyMeCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("replyme", "Повтор вашего сообщения");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Сообщение для отправки пользователю
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Отписка по умолчанию, если <see cref="Message"/> был empty или null
    /// </summary>
    public string DefaultMessage => "Вы ничего не отправили";
}

internal class ReplyMeCommandHandler : TelegramRequestHandler<ReplyMeCommand> {

    public ReplyMeCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override async Task<Unit> Handle(ReplyMeCommand request, BotClient botClient, CancellationToken cancellationToken) {
        await botClient.SendMessageAsync(request.PrivateChatId, string.IsNullOrEmpty(request.Message) ? request.DefaultMessage : request.Message, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}

internal class ReplyMeCommandParamsBuilder : ITelegramRequestParamsBuilder<ReplyMeCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref ReplyMeCommand request) {
        request.PrivateChatId = update.Message!.Chat!.Id;
        request.Message = string.Join(" ", args) ?? "<empty>";
    }
}