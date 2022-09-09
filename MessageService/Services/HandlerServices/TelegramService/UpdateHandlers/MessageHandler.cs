using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers {

    /// <summary>
    /// Обработчик, предназченый для распознования и ответа на действия, которые поступили из чата телеграма
    /// </summary>
    public class CommandHandler {
        private ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(ILogger<CommandHandler> logger, IServiceProvider serviceProvider) {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
            if (update.Message == null) {
                _logger.LogInformation($"Сообщение было пусто");
                return Task.CompletedTask;
            }

            switch (update.Message.Type) {
                case MessageType.Text:
                    var msgtxt = update.Message?.Text?.Split(' ').First() ?? String.Empty;

                    if (string.IsNullOrEmpty(msgtxt) || msgtxt.First().Equals('/') == false) {
                        break;
                    }

                    var commands = _serviceProvider.GetServices<BotCommandAction>();
                    var command = commands.FirstOrDefault(e => e.Command.Equals(msgtxt));

                    if (command != null) {
                        update.Message.Text = String.Join(" ", update.Message.Text.Split(' ').Skip(1));
                        command.ExecuteAction(botClient, update, cancellationToken);
                    }
                    else {
                        _logger.LogInformation("Not supported command: " + update.Message.Text);
                    }

                    break;

                default:
                    _logger.LogInformation("Not supported type of message: " + update.Message.Type);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}