using Application.Common.Models;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface ISenderMessage {

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение для отправки</param>
    /// <param name="chat">Чат, в который отправить сообщение</param>
    public Task SendMessageAsync(MessageDTO message, Chat chat);

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение для отправки</param>
    /// <param name="chats">Чаты, в который отправить сообщение</param>
    public Task SendMessageAsync(MessageDTO message, IEnumerable<Chat> chats);
}