using MessageService.Datas;
using MessageService.Services.HelperService;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.ChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто отсоединился от чата.
/// Если отсоедилнился бот, то забываем о существовании этого чата
/// </summary>
public class ForgetChat {
    private readonly IDatabaseService<DataContext> _dbService;
    private readonly ILogger<ForgetChat> _logger;
    private readonly IWhoIam _whoIam;

    public ForgetChat(IWhoIam whoIam, IDatabaseService<DataContext> dbService, ILogger<ForgetChat> logger) {
        _dbService = dbService;
        _whoIam = whoIam;
        _logger = logger;
    }

    internal async Task Execute(Message message) {
        var leftMember = message.LeftChatMember!;
        var iam = await _whoIam.GetMeAsync();
        if (leftMember.Id == iam.Id) {
            var chatId = message.Chat.Id;

            var context = _dbService.GetDBContext();

            var findedChat = context.Chats.FirstOrDefault(e => e.ChatId.Equals(chatId.ToString()));

            if (findedChat != null) {
                context.Entry(findedChat).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                await context.SaveChangesAsync();
                _logger.LogInformation($"Меня выгнали из группы {findedChat.Name} :с");
            }
        }
    }
}