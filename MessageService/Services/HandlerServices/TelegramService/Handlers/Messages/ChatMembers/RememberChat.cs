using MessageService.Datas;
using MessageService.Services.HelperService;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.ChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто присоединился.
/// </summary>
public class RememberChat {
    private readonly IDatabaseService<DataContext> _context;
    private readonly ITelegramHandlerService _telegramHandlerService;

    public RememberChat(ITelegramHandlerService telegramHandlerService, IDatabaseService<DataContext> context) {
        _context = context;
        _telegramHandlerService = telegramHandlerService;
    }

    internal async Task Execute(Message message) {
        var me = await _telegramHandlerService.GetMeAsync();

        // Если подключившимся является "Я", то добавляем чат в бд
        if (message.From != null && message.From.Id.Equals(me.Id)) {
        }
    }
}