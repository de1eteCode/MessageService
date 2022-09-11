using MessageService.Datas;
using MessageService.Services.HelperService;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.ChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто отсоединился от чата.
/// Если отсоедилнился бот, то забываем о существовании этого чата
/// </summary>
public class ForgetChat {
    private readonly IDatabaseService<DataContext> _context;

    public ForgetChat(IDatabaseService<DataContext> context) {
        _context = context;
    }

    internal Task Execute(Message message) {
        throw new NotImplementedException();
    }
}