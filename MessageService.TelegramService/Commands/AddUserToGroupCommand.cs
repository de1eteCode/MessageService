using Application.Groups.Queries;
using Application.UserGroups.Commands;
using Application.Users.Queries;
using Domain.Models;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record AddUserToGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("addusertogroup", "Добавление пользователя в группу");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор пользователя ТГ для добавления в группу
    /// </summary>
    public long? UserTgIdToAdd { get; set; }

    /// <summary>
    /// Альтернативный идентификатор <see cref="Group"/>
    /// </summary>
    public int? GroupAlternativeIdToAdd { get; set; }
}

internal class AddUserToGroupCommandHandler : TelegramRequestHandler<AddUserToGroupCommand> {
    private readonly IMediator _mediator;

    public AddUserToGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(AddUserToGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.UserTgIdToAdd == null || request.GroupAlternativeIdToAdd == null) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = (long)request.UserTgIdToAdd }, cancellationToken);
        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = (int)request.GroupAlternativeIdToAdd }, cancellationToken);

        if (user == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь с идентификатором {(long)request.UserTgIdToAdd} не найден", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (group == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Группа с идентификатором {(int)request.GroupAlternativeIdToAdd} не найдена", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (group.UserGroups.Any(e => e.UserUID.Equals(user.UID))) {
            // уже имеется
            await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь {user.Name} уже состоит в группе {group.Name}", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var userToGroup = await _mediator.Send(new CreateUserGroupCommand() {
            GroupUID = group.UID,
            UserUID = user.UID
        }, cancellationToken);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь {user.Name} добавлен в группу {group.Name}", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис для добавления пользователя в группу: /addusertogroup [id группы] [tg id пользователя]",
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}

internal class AddUserToGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<AddUserToGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref AddUserToGroupCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 2) {
            return;
        }

        if (long.TryParse(args.ElementAt(0), out var userTgId)) {
            request.UserTgIdToAdd = userTgId;
        }

        if (int.TryParse(args.ElementAt(1), out var groupAltId)) {
            request.GroupAlternativeIdToAdd = groupAltId;
        }
    }
}