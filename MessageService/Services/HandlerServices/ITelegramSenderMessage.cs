using RepositoryLibrary.Models;

namespace MessageService.Services.HandlerServices;

public interface ITelegramSenderMessage {

    /// <summary>
    /// Отправка сообщения в телеграмме
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="chat">Чат, в который будет отправлено сообщение</param>
    /// <param name="try">Количество попыток отправки, по умолчанию 1</param>
    public Task SendMessageAsync(string message, Chat chat, int @try = 1);
}