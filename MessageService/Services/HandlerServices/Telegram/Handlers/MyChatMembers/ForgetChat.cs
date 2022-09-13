using MessageService.Datas;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.MyChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто отсоединился от чата.
/// Если отсоедилнился бот, то забываем о существовании этого чата
/// </summary>
public class ForgetChat {
    private readonly IDatabaseService<DataContext> _dbService;
    private readonly ILogger<ForgetChat> _logger;

    public ForgetChat(IDatabaseService<DataContext> dbService, ILogger<ForgetChat> logger) {
        _dbService = dbService;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(ChatMemberUpdated chatMemberUpdate) {
        var context = _dbService.GetDBContext();

        // проверка на чат
        var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId!.Equals(chatMemberUpdate.Chat.Id.ToString()));

        if (chat != null) {
            // бот знает о чате и надо пометить что его кикнули
            chat.IsJoined = false;
            chat.KickedTime = chatMemberUpdate.Date;
            chat.KickedByUserLogin = chatMemberUpdate.From?.Username ?? "unknown user";
            context.Entry(chat).State = EntityState.Modified;
        }
        else {
            // бот не знал о чате, на всякий случай запомним чат
            chat = new Datas.Models.Chat() {
                ChatId = chatMemberUpdate.Chat.Id.ToString(),
                Name = chatMemberUpdate.Chat.Title,
                IsJoined = false,
                KickedByUserLogin = chatMemberUpdate.From?.Username ?? "unknown user",
                KickedTime = chatMemberUpdate.Date
            };
            context.Entry(chat).State = EntityState.Added;
        }

        await context.SaveChangesAsync();
        _logger.LogInformation($"Меня выгнали из группы {chat.Name}");
    }
}