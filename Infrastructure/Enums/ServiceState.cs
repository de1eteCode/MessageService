namespace Infrastructure.Enums;

/// <summary>
/// Отображение состояния сервиса
/// </summary>
public enum ServiceState {

    /// <summary>
    /// Сервис включен
    /// </summary>
    Online,

    /// <summary>
    /// Сервис выключен
    /// </summary>
    Offline,

    /// <summary>
    /// Сервис в состоянии ошибки
    /// </summary>
    FatalError
}