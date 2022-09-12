namespace MessageService.Services.HandlerServices.Telegram;

/// <summary>
/// Результат валидации
/// </summary>
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