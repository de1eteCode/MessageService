using Application.Chats.Commands;
using Application.Chats.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record ForgetChatCommand : ITelegramPassiveRequest {
    public BotCommand BotCommand => new BotCommand("forgetchat", "Запоминание чата ботом");
    public ChatMemberUpdated ChatMemberUpdate { get; set; } = default!;
}

internal class ForgetChatCommandHandler : TelegramPassiveRequestHandler<ForgetChatCommand> {
    private readonly IMediator _mediator;

    public ForgetChatCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(ForgetChatCommand request, BotClient botClient, CancellationToken cancellationToken) {
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

internal class ForgetChatCommandParamsBuilder : ITelegramPassiveRequestParamsBuilder<ForgetChatCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref ForgetChatCommand request) {
        request.ChatMemberUpdate = update.MyChatMember!;
    }
}