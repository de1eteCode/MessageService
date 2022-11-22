using Application.Chats.Commands.CreateChat;
using Application.Chats.Commands.UpdateChat;
using Application.Chats.Queries.GetChat;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record RememberChatCommand : ITelegramPassiveRequest {
    public BotCommand BotCommand => new BotCommand("forgetchat", "Запоминание чата ботом");
    public ChatMemberUpdated ChatMemberUpdate { get; set; } = default!;
}

internal class RememberChatCommandHandler : TelegramPassiveRequestHandler<RememberChatCommand> {
    private readonly IMediator _mediator;

    public RememberChatCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(RememberChatCommand request, BotClient botClient, CancellationToken cancellationToken) {
        var chat = await _mediator.Send(new GetChatCommand() { TelegramChatId = request.ChatMemberUpdate.Chat.Id });

        IRequest<Domain.Models.Chat> command = default!;

        if (chat == null) {
            // чат не существует в бд
            command = new CreateChatCommand() {
                TelegramChatId = request.ChatMemberUpdate.Chat!.Id,
                Name = request.ChatMemberUpdate.Chat!.Title!,
                IsJoined = false,
                KickedUserLogin = request.ChatMemberUpdate.From?.Username ?? "unknown user",
                KickedUserId = request.ChatMemberUpdate.From?.Id ?? -1,
                Time = DateTimeHelper.ConvertUnixToDateTime(request.ChatMemberUpdate.Date)
            };
        }
        else {
            // чат существует в бд
            command = new UpdateChatCommand() {
                UID = chat.UID,
                TelegramChatId = request.ChatMemberUpdate.Chat!.Id,
                Name = request.ChatMemberUpdate.Chat!.Title!,
                IsJoined = false,
                KickedUserLogin = request.ChatMemberUpdate.From?.Username ?? "unknown user",
                KickedUserId = request.ChatMemberUpdate.From?.Id ?? -1,
                Time = DateTimeHelper.ConvertUnixToDateTime(request.ChatMemberUpdate.Date)
            };
        }

        var result = await _mediator.Send(command);

        return Unit.Value;
    }
}

internal class RememberChatCommandParamsBuilder : ITelegramPassiveRequestParamsBuilder<RememberChatCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref RememberChatCommand request) {
        request.ChatMemberUpdate = update.MyChatMember!;
    }
}