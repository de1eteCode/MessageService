using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.Commands;

public abstract class BotCommandAction : BotCommand {

    protected BotCommandAction(string command, string description) {
        if (string.IsNullOrEmpty(command)) {
            throw new ArgumentNullException(nameof(command));
        }

        if (string.IsNullOrEmpty(description)) {
            throw new ArgumentException(nameof(description));
        }

        if (command.First().Equals('/') == false) {
            command = '/' + command;
        }

        if (command.Length < 1 || command.Length > 32) {
            throw new ArgumentException(nameof(command) + " вне диапазона по количеству символов");
        }

        if (description.Length < 3 || description.Length > 256) {
            throw new ArgumentException(nameof(description) + " вне диапазона по количеству символов");
        }

        Command = command;
        Description = description;
    }

    /// <summary>
    /// Вызов обработки команды
    /// </summary>
    /// <param name="botClient">Телеграм клиент бота</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public abstract Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
}