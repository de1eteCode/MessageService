using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram;

public interface IWhoIam {

    /// <summary>
    /// Информация о боте
    /// </summary>
    public Task<User> GetMeAsync();
}