using Application.Roles.Queries.GetRoles;
using Application.Users.Commands.CreateUser;
using Application.Users.Queries.GetUser;
using Domain.Models;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

internal record AddUserCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("adduser", "Добавление пользователя");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Идентификатор пользователя ТГ для добавления
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// Альтернативный идентификатор <see cref="Role"/>
    /// </summary>
    public int? AlternativeRoleId { get; set; }

    /// <summary>
    /// Имя пользователя в системе
    /// </summary>
    public string? UserName { get; set; }
}

internal class AddUserCommandHandler : TelegramRequestHandler<AddUserCommand> {
    private readonly IMediator _mediator;

    public AddUserCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(AddUserCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.CreateUserId == null || request.AlternativeRoleId == null || string.IsNullOrEmpty(request.UserName)) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var user = await _mediator.Send(new GetUserCommand() { TelegramId = (long)request.CreateUserId }, cancellationToken);

        if (user != null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь {user.Name} ({user.TelegramId}) был ранее добавлен", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var role = await _mediator.Send(new GetRoleByAlternativeIdCommand() { AlternativeId = (int)request.AlternativeRoleId }, cancellationToken);

        if (role == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"Я не нашел роль под id ", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var newUser = await _mediator.Send(new CreateUserCommand() {
            TelegramId = (long)request.CreateUserId,
            Name = request.UserName!,
            RoleUID = role.UID
        }, cancellationToken);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Пользователь {newUser.Name} был успешно добавлен с ролью {newUser.Role.Name}", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        var roles = await _mediator.Send(new GetRolesCommand(), cancellationToken);

        var rolesStrCollection = roles
            .OrderBy(e => e.AlternativeId)
            .Select(e => string.Format("{0}. {1}", e.AlternativeId, e.Name))
            .ToList();

        await _botClient.SendMessageAsync(privateChatId,
            "Синтаксис для добавления пользователя: /adduser [id роль] [id telegram] [Имя]\n" +
            "Доступные роли:\n" +
            String.Join("\n", rolesStrCollection),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class AddUserCommandParamsBuilder : ITelegramRequestParamsBuilder<AddUserCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref AddUserCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 3) {
            return;
        }

        if (int.TryParse(args.ElementAt(0), out var idRole)) {
            request.AlternativeRoleId = idRole;
        }

        if (long.TryParse(args.ElementAt(1), out var tgId)) {
            request.CreateUserId = tgId;
        }

        request.UserName = string.Join(" ", args.Skip(2).Take(3));
    }
}