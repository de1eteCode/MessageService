using MessageService.Services.HandlerServices.Telegram.Handlers.MyChatMembers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.Telegram.Handlers;

/// <summary>
/// Обработчик, отвечающий за события добавления/исключения бота в чатах
/// </summary>
public class MyChatMemberHandler : IUpdateHandler<ChatMemberUpdated> {
    private readonly ILogger<MyChatMemberHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MyChatMemberHandler(ILogger<MyChatMemberHandler> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, ChatMemberUpdated myChatMember, CancellationToken cancellationToken) {
        // надстройка, чтобы обновления о изменениях бота работали только в группах
        if (myChatMember.Chat.Type != ChatType.Group) {
            return;
        }

        switch (myChatMember.NewChatMember.Status) {
            case ChatMemberStatus.Creator:
            case ChatMemberStatus.Administrator:
            case ChatMemberStatus.Member:
            case ChatMemberStatus.Restricted:
                await _serviceProvider.GetService<RememberChat>()!.ExecuteActionAsync(myChatMember);
                break;

            case ChatMemberStatus.Left:
            case ChatMemberStatus.Kicked:
                await _serviceProvider.GetService<ForgetChat>()!.ExecuteActionAsync(myChatMember);
                break;

            default:
                _logger.LogInformation("Not supported type of MyChatMember: " + myChatMember.NewChatMember.Status);
                break;
        }
    }
}