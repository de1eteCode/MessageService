using MediatR;
using Domain.Models;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.AvailableMethods;
using Application.Roles.Queries;
using Application.Users.Queries;
using Application.Users.Commands;

namespace MessageService.TelegramService.Commands;

internal record ChangeUserCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("changeuser", "Изменение пользователя");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор пользователя ТГ для изменения
    /// </summary>
    public long? ChangeUserTgId { get; set; }

    /// <summary>
    /// Альтернативный идентификатор <see cref="Role"/>
    /// </summary>
    public int? NewRoleAlternativeId { get; set; }
}

internal class ChangeUserCommandHandler : TelegramRequestHandler<ChangeUserCommand> {
    private readonly IMediator _mediator;

    public ChangeUserCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(ChangeUserCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.ChangeUserTgId == null || request.NewRoleAlternativeId == null) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = (long)request.ChangeUserTgId }, cancellationToken);
        var role = await _mediator.Send(new GetRoleByAlternativeIdCommand() { AlternativeId = (int)request.NewRoleAlternativeId }, cancellationToken);

        if (user == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь с идентификатором {(long)request.ChangeUserTgId} не найден", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        if (role == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Роль с идентификатором {(int)request.NewRoleAlternativeId} не найдена", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        user = await _mediator.Send(new UpdateUserCommand() {
            UserUID = user.UID,
            RoleUID = role.UID,
        }, cancellationToken);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь {user.Name} был успешно изменен", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        var roles = await _mediator.Send(new GetRolesCommand(), cancellationToken);

        var rolesStrCollection = roles
            .OrderBy(e => e.AlternativeId)
            .Select(e => string.Format("{0}. {1}", e.AlternativeId, e.Name))
            .ToList();

        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис изменения пользователя: /changeuser [tg user id] [id роли]\n" +
                "Доступные роли:\n" +
                String.Join("\n", rolesStrCollection),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class ChangeUserCommandParamsBuilder : ITelegramRequestParamsBuilder<ChangeUserCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref ChangeUserCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 2) {
            return;
        }

        if (long.TryParse(args.ElementAt(0), out var userTgId)) {
            request.ChangeUserTgId = userTgId;
        }

        if (int.TryParse(args.ElementAt(1), out var roleAltId)) {
            request.NewRoleAlternativeId = roleAltId;
        }
    }
}