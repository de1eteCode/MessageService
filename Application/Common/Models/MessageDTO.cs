namespace Application.Common.Models;

/// <summary>
/// DTO модель сообщения для отправки
/// </summary>
public class MessageDTO {

    /// <summary>
    /// Тема сообщения
    /// </summary>
    public string Subject { get; set; } = default!;

    /// <summary>
    /// Тело сообщения
    /// </summary>
    public string Body { get; set; } = default!;
}