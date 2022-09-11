using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.Commands;

public class StartCommand : BotCommandAction {
    private readonly ILogger<StartCommand> _logger;

    public StartCommand(ILogger<StartCommand> logger) : base("start", "Инициализация бота") {
        _logger = logger;
    }

    public override Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var sender = message.From;
        var typeChat = message.Chat.Type;
        _logger.LogInformation($"Вызвана инициализация бота. Пользователь: {sender?.Username ?? "user name null"}");
        return Task.CompletedTask;
    }
}