using DataLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.MyChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто присоединился.
/// </summary>
public class RememberChat {
    private readonly IDatabaseService<DataContext> _dbService;
    private readonly ILogger<RememberChat> _logger;

    public RememberChat(IDatabaseService<DataContext> dbService, ILogger<RememberChat> logger) {
        _dbService = dbService;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(ChatMemberUpdated chatMemberUpdate) {
        var context = _dbService.GetDBContext();

        // проверка на ранее добавления бота в чат
        var chat = await context.Chats.SingleOrDefaultAsync(e => e.TelegramChatId!.Equals(chatMemberUpdate.Chat.Id));

        if (chat != null) {
            // бот ранее состоял в этом чате
            chat.IsJoined = true;
            chat.Name = chatMemberUpdate.Chat.Title!;
            chat.KickedTime = null;
            chat.KickedByUserLogin = null;
            chat.KickedByUserId = null;
            context.Entry(chat).State = EntityState.Modified;
        }
        else {
            // бот впервые в этом чате
            chat = new DataLibrary.Models.Chat() {
                IsJoined = true,
                TelegramChatId = chatMemberUpdate.Chat.Id,
                Name = chatMemberUpdate.Chat.Title!
            };

            context.Entry(chat).State = EntityState.Added;
        }

        _logger.LogInformation($"Меня добавили в чат \"{chat.Name}\"");

        await context.SaveChangesAsync();
    }
}