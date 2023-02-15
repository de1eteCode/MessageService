using Application.Common.Models;
using Domain.Entities;

namespace Application.Common.Interfaces;

/// <summary>
/// Сервис по отправке сообщений
/// </summary>
public interface ISenderMessageService {

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