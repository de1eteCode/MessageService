using Application.ChatGroups.Commands;
using Application.Chats.Queries;
using Application.Groups.Queries;
using Application.Users.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record RemoveChatFromGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("removechatfromgroup", "Удаление чата из группы");

    /// <summary>
    /// Идентификатор пользователя ТГ
    /// </summary>
    public long SenderUserId { get; set; }

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор чата для удаления из группы
    /// </summary>
    public long? ChatIdToRemove { get; set; } = null;

    /// <summary>
    /// Альтернативный идентификатор <see cref="Domain.Models.Group"/>, в которую добавляется чат
    /// </summary>
    public int? GroupAlternativeId { get; set; } = null;
}

internal class RemoveChatFromGroupCommandHandler : TelegramRequestHandler<RemoveChatFromGroupCommand> {
    private readonly IMediator _mediator;

    public RemoveChatFromGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(RemoveChatFromGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.ChatIdToRemove == null || request.GroupAlternativeId == null) {
            return await SendDefaultMessageAsync(request.PrivateChatId, cancellationToken);
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = request.SenderUserId }, cancellationToken);
        var chat = await _mediator.Send(new GetChatCommand() { TelegramChatId = (long)request.ChatIdToRemove }, cancellationToken);
        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = (int)request.GroupAlternativeId }, cancellationToken);

        if (user == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, "Странно, я не нашел твою учетку в своей базе данных", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (chat == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Я не знаю о чате с идентификатором {request.ChatIdToRemove}", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (group == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"У меня нет группы с идентификатором {request.GroupAlternativeId}", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        // проверка пользователя на наличие в группе
        if (group.UserGroups!.Any(e => e.UserUID == user.UID) == false) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Ты не можешь удалять чаты из группы, в которой не состоишь", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        // проверка наличия чата в группе
        var chatToGroup = group.ChatGroups.FirstOrDefault(e => e.ChatUID!.Equals(chat.UID) && e.GroupUID == group.UID);

        // если имеется
        if (chatToGroup != null && chatToGroup.IsDeleted == false) {
            chatToGroup = await _mediator.Send(new UpdateChatGroupCommand() {
                ChatGroupUID = chat.UID,
                IsDeleted = true
            }, cancellationToken);
            return Unit.Value;
        }

        await botClient.SendMessageAsync(request.PrivateChatId, $"Чат \"{chat.Name}\" не состоит в группе \"{group.Name}\"", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessageAsync(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис удаления чата из группы: /removechatfromgroup [id чата] [id группы]",
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}

internal class RemoveChatFromGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<RemoveChatFromGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref RemoveChatFromGroupCommand request) {
        request.PrivateChatId = update.Message!.Chat!.Id;
        request.SenderUserId = update.Message!.From!.Id;

        if (args.Count() < 2) {
            return;
        }

        // парсинг идентификатора чата
        if (long.TryParse(args.ElementAt(0), out long chatIdToAdd)) {
            request.ChatIdToRemove = chatIdToAdd;
        }

        // парсинг идентификатора группы
        if (int.TryParse(args.ElementAt(1), out int groupIdToAdd)) {
            request.GroupAlternativeId = groupIdToAdd;
        }
    }
}