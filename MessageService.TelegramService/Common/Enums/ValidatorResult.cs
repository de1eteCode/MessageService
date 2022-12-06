namespace MessageService.TelegramService.Common.Enums;

/// <summary>
/// Результат валидации
/// </summary>
internal enum ValidatorResult {

    /// <summary>
    /// Позволено
    /// </summary>
    Allow = 0x1,

    /// <summary>
    /// Запрещено
    /// </summary>
    Deny = 0x2,

    /// <summary>
    /// Все равно
    /// </summary>
    Anyway = 0x4
}