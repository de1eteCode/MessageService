using Telegram.Bot.Types;

namespace TelegramService.Interfaces;

internal interface IWhoIam {

    /// <summary>
    /// Информация о боте
    /// </summary>
    public Task<User> GetMeAsync();
}