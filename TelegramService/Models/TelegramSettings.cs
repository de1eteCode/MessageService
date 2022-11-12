namespace TelegramService.Models;

internal class TelegramSettings {
    public string Token { get; set; } = default!;
    public int LimitRequests { get; set; }
}