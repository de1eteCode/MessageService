using Application.Groups.Commands;
using Application.Users.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record AddGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("addgroup", "Добавление новой группы");

    /// <summary>
    /// Идентификатор пользователя ТГ
    /// </summary>
    public long SenderUserId { get; set; }

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Наименование новой группы
    /// </summary>
    public string? GroupName { get; set; }
}

internal class AddGroupCommandHandler : TelegramRequestHandler<AddGroupCommand> {
    private readonly IMediator _mediator;

    public AddGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(AddGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.GroupName)) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = request.SenderUserId }, cancellationToken);

        if (user == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, "Странно, я не нашел твоей учетной записи у себя в базе", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var group = await _mediator.Send(new CreateGroupCommand() {
            Name = request.GroupName,
            OwnerUserUID = user.UID
        }, cancellationToken);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Группа \"{group.Name}\" успешно добавлена под идентификатором: {group.AlternativeId}", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис добавления новой группы: /addgroup [наименование группы]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class AddGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<AddGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref AddGroupCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;
        request.SenderUserId = update.Message!.From!.Id;

        if (args.Any()) {
            request.GroupName = args.First();
        }
    }
}