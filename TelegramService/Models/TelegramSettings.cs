namespace TelegramService.Models;

/// <summary>
/// Конфигурационные параметры для работы с Telegram api
/// </summary>
public class TelegramSettings {

    /// <summary>
    /// Токен бота для доступа к api
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Ограничение по запросам к api
    /// </summary>
    public int LimitRequests { get; set; }
}