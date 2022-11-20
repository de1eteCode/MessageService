namespace MessageService.TelegramService.Common.Models;

public class TelegramBotSettings {
    public string Token { get; init; } = default!;
    public bool IgnoreBotExceptions { get; init; } = default!;
}