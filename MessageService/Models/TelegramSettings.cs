namespace MessageService.Models;

public class TelegramSettings {
    public string Token { get; set; } = default!;
    public int LimitRequests { get; set; }
}