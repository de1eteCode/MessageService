using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Commands;
internal record ReplyMeCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("replyme", "Повтор вашего сообщения");
    public long PrivateChatId { get; set; }
    public string? Message { get; set; }
}

internal class ReplyMeCommandHandler : TelegramRequestHandler<ReplyMeCommand> {

    public ReplyMeCommandHandler(BotClient botClient) : base(botClient) {
    }

    public override async Task<Unit> Handle(ReplyMeCommand request, BotClient botClient, CancellationToken cancellationToken) {
        await botClient.SendMessageAsync(request.PrivateChatId, request.Message ?? "Empty", cancellationToken: cancellationToken);
        return Unit.Value;
    }
}