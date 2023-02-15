using Application.Groups.Commands.CreateGroup;
using Application.Users.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Добавление новой группы
/// </summary>
internal class AddGroupCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public AddGroupCommand(IMediator mediator) : base("addgroup", "Добавление новой группы") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var chatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await botClient.SendTextMessageAsync(chatId, "Синтаксис добавления новой группы: /addgroup [наименование группы]");
            return;
        }

        var addedGroupUser = await _mediator.Send(new GetUserCommand() { TelegramId = message.From!.Id });

        if (addedGroupUser == null) {
            await botClient.SendTextMessageAsync(chatId, "Странно, я не нашел твоей учетной записи у себя в базе");
            return;
        }

        var newGroup = await _mediator.Send(new CreateGroupCommand() {
            Name = msg,
            OwnerUserUID = addedGroupUser.UID
        });

        await botClient.SendTextMessageAsync(chatId, $"Группа \"{newGroup.Name}\" успешно добавлена под идентификатором: {newGroup.AlternativeId}");
    }
}