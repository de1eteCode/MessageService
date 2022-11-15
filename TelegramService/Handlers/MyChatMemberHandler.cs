using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramService.Interfaces;
using TelegramService.PassiveCommands;

namespace TelegramService.Handlers;

/// <summary>
/// Обработчик, отвечающий за события добавления/исключения бота в чатах
/// </summary>
internal class MyChatMemberHandler : IUpdateHandler<ChatMemberUpdated> {
    private readonly ILogger<MyChatMemberHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MyChatMemberHandler(ILogger<MyChatMemberHandler> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, ChatMemberUpdated myChatMember, CancellationToken cancellationToken) {
        // надстройка, чтобы обновления о изменениях бота работали только в группах и приватных группах
        if (myChatMember.Chat.Type != ChatType.Group && myChatMember.Chat.Type != ChatType.Supergroup) {
            return;
        }

        switch (myChatMember.NewChatMember.Status) {
            case ChatMemberStatus.Creator:
            case ChatMemberStatus.Administrator:
            case ChatMemberStatus.Member:
            case ChatMemberStatus.Restricted:
                await _serviceProvider.GetService<RememberChat>()!.ExecuteActionAsync(myChatMember);
                await botClient.SendTextMessageAsync(722270819, $"Меня добавили в \"{myChatMember.Chat.Title}\"");
                break;

            case ChatMemberStatus.Left:
            case ChatMemberStatus.Kicked:
                await _serviceProvider.GetService<ForgetChat>()!.ExecuteActionAsync(myChatMember);
                await botClient.SendTextMessageAsync(722270819, $"Меня выгнали из \"{myChatMember.Chat.Title}\"");
                break;

            default:
                _logger.LogInformation("Not supported type of MyChatMember: " + myChatMember.NewChatMember.Status);
                break;
        }
    }
}