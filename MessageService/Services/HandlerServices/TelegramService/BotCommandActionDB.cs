using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService;

public abstract class BotCommandActionDB : BotCommandAction {
    private readonly IServiceScopeFactory _serviceScopeFactory;

    protected BotCommandActionDB(IServiceScopeFactory serviceScopeFactory, string command, string description) : base(command, description) {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<DataContext>() ?? throw new ArgumentNullException();
        ExecuteAction(botClient, update, cancellationToken, context);
    }

    /// <summary>
    /// Вызов обработки команды
    /// </summary>
    /// <param name="botClient">Телеграм клиент бота</param>
    /// <param name="update">Обновление</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <param name="dataContext">Экземпляр базы данных</param>
    public abstract void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, DataContext dataContext);
}