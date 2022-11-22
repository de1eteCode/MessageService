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
    public long PrivateChatId { get; set; }
    public string? Message { get; set; }
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