namespace MessageService.Services.HandlerServices;

public interface IHandlerService {
    ///// <summary>
    ///// Отправка сообщения
    ///// </summary>
    ///// <param name="message">Текст сообщения</param>
    //public void SendMessage(string message);

    /// <summary>
    /// Запуск прослушивания
    /// </summary>
    public void StartListener();

    /// <summary>
    /// Остановка прослушивания
    /// </summary>
    public void StopListener();
}