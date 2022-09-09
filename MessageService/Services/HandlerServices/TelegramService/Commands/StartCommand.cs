using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands;

public class StartCommand : BotCommandActionDB {
    private readonly ILogger<BotCommandAction> _logger;

    public StartCommand(ILogger<BotCommandAction> logger, IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory, "start", "Инициализация бота") {
        _logger = logger;
    }

    public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, DataContext dataContext) {
        var sender = update.Message.From;
        var typeChat = update.Message.Chat.Type;
        _logger.LogInformation("Вызвана инициализация бота");
    }
}