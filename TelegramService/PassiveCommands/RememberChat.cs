using Application.Chats.Commands.CreateChat;
using Application.Chats.Commands.UpdateChat;
using Application.Chats.Queries.GetChat;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace TelegramService.PassiveCommands;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто присоединился.
/// </summary>
internal class RememberChat {
    private readonly IMediator _mediator;
    private readonly ILogger<RememberChat> _logger;

    public RememberChat(IMediator mediator, ILogger<RememberChat> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(ChatMemberUpdated chatMemberUpdate) {
        var mediator_chat = await _mediator.Send(new GetChatCommand() { TelegramChatId = chatMemberUpdate.Chat.Id });

        IRequest<Domain.Models.Chat> command = default!;

        if (mediator_chat == null) {
            // чат не существует в бд
            command = new CreateChatCommand() {
                TelegramChatId = chatMemberUpdate.Chat!.Id,
                Name = chatMemberUpdate.Chat!.Title!,
                IsJoined = false,
                KickedUserLogin = chatMemberUpdate.From?.Username ?? "unknown user",
                KickedUserId = chatMemberUpdate.From?.Id ?? -1,
                Time = chatMemberUpdate.Date
            };
        }
        else {
            // чат существует в бд
            command = new UpdateChatCommand() {
                UID = mediator_chat.UID,
                TelegramChatId = chatMemberUpdate.Chat!.Id,
                Name = chatMemberUpdate.Chat!.Title!,
                IsJoined = false,
                KickedUserLogin = chatMemberUpdate.From?.Username ?? "unknown user",
                KickedUserId = chatMemberUpdate.From?.Id ?? -1,
                Time = chatMemberUpdate.Date
            };
        }

        var result = await _mediator.Send(command);

        _logger.LogInformation($"Меня добавили в чат \"{chatMemberUpdate.Chat!.Title}\"");
    }
}