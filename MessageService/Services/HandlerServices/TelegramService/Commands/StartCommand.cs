using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands;

public class StartCommand : BotCommandAction {
    private readonly ILogger<BotCommandAction> _logger;

    public StartCommand(ILogger<BotCommandAction> logger)
        : base("start", "Инициализация бота") {
        _logger = logger;
    }

    public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        _logger.LogInformation("Вызвана инициализация бота");
    }
}