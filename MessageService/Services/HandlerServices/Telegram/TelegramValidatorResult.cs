namespace MessageService.Services.HandlerServices.Telegram;

public enum TelegramValidatorResult {

    /// <summary>
    /// Позволено
    /// </summary>
    Allow,

    /// <summary>
    /// Запрещено
    /// </summary>
    Deny,

    /// <summary>
    /// Все равно
    /// </summary>
    Anyway
}