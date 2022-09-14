using MessageService.Services.HandlerServices.Telegram.Attributes;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Команда выхода бота из чата
/// </summary>
[TelegramUserRole("Системный администратор")]
public class LeaveChatByIdCommand : BotCommandAction {
    private readonly IWhoIam _whoIam;

    public LeaveChatByIdCommand(IWhoIam whoIam) : base("leavechatbyid", "Команда для выхода из чата ботом") {
        _whoIam = whoIam;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы не отправили мне id чата, из которого я должен выйти");
            return;
        }

        if (long.TryParse(msg, out long id)) {
            try {
                var chatId = new ChatId(id);
                var chatInfo = await botClient.GetChatAsync(chatId).WaitAsync(new TimeSpan(hours: 0, minutes: 0, seconds: 10));

                var iamIsMember = await botClient.GetChatMemberAsync(chatInfo.Id, (await _whoIam.GetMeAsync()).Id) != null; // Генерирует ошибку с ErrCode = 403

                if (iamIsMember) {
                    await botClient.LeaveChatAsync(chatId);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Я вышел из чата {chatInfo.Title} ({chatInfo.Id})");
                }
            }
            catch (ApiRequestException api) {
                await botClient.SendTextMessageAsync(message.Chat.Id, GetMsgError(api));
            }
            catch (AggregateException aggregate) {
                var exs = aggregate.InnerException;
                var msgEx = exs switch {
                    ApiRequestException api => GetMsgError(api),
                    _ => exs?.Message ?? "Какая-то неизвестная ошибка"
                };

                await botClient.SendTextMessageAsync(message.Chat.Id, msgEx);
            }
        }
        else {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"{msg} - не похож на id чата");
        }
    }

    private string GetMsgError(ApiRequestException api) => api.ErrorCode switch {
        400 => $"Хм, я не нашел чат", // Bad request: Not found chat
        403 => $"Я не состаю в этом чате", // Forbidden: bot is not a member of the group chat
        _ => api.Message
    };
}