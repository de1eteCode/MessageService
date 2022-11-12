using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace TelegramService.PassiveCommands;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто присоединился.
/// </summary>
internal class RememberChat {
    private readonly IDataContext _context;
    private readonly ILogger<RememberChat> _logger;

    public RememberChat(IDataContext context, ILogger<RememberChat> logger) {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(ChatMemberUpdated chatMemberUpdate) {
        // проверка на ранее добавления бота в чат
        var chat = await _context.Chats.SingleOrDefaultAsync(e => e.TelegramChatId!.Equals(chatMemberUpdate.Chat.Id));

        if (chat != null) {
            // бот ранее состоял в этом чате
            chat.IsJoined = true;
            chat.Name = chatMemberUpdate.Chat.Title!;
            chat.KickedTime = null;
            chat.KickedByUserLogin = null;
            chat.KickedByUserId = null;
            _context.Entry(chat).State = EntityState.Modified;
        }
        else {
            // бот впервые в этом чате
            chat = new Domain.Models.Chat() {
                IsJoined = true,
                TelegramChatId = chatMemberUpdate.Chat.Id,
                Name = chatMemberUpdate.Chat.Title!
            };

            _context.Entry(chat).State = EntityState.Added;
        }

        _logger.LogInformation($"Меня добавили в чат \"{chat.Name}\"");

        await _context.SaveChangesAsync();
    }
}