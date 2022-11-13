using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace TelegramService.PassiveCommands;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто отсоединился от чата.
/// Если отсоедилнился бот, то забываем о существовании этого чата
/// </summary>
internal class ForgetChat {
    private readonly IDataContext _context;
    private readonly ILogger<ForgetChat> _logger;

    public ForgetChat(IDataContext context, ILogger<ForgetChat> logger) {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(ChatMemberUpdated chatMemberUpdate) {
        // проверка на чат
        var chat = await _context.Chats.SingleOrDefaultAsync(e => e.TelegramChatId!.Equals(chatMemberUpdate.Chat.Id));

        if (chat != null) {
            // бот знает о чате и надо пометить что его кикнули
            chat.IsJoined = false;
            chat.KickedTime = chatMemberUpdate.Date;
            chat.KickedByUserLogin = chatMemberUpdate.From?.Username ?? "unknown user";
            chat.KickedByUserId = chatMemberUpdate.From?.Id ?? -1;
            _context.Entry(chat).State = EntityState.Modified;
        }
        else {
            // бот не знал о чате, на всякий случай запомним чат
            chat = new Domain.Models.Chat() {
                TelegramChatId = chatMemberUpdate.Chat.Id,
                Name = chatMemberUpdate.Chat!.Title!,
                IsJoined = false,
                KickedByUserLogin = chatMemberUpdate.From?.Username ?? "unknown user",
                KickedByUserId = chatMemberUpdate.From?.Id ?? -1,
                KickedTime = chatMemberUpdate.Date
            };
            _context.Entry(chat).State = EntityState.Added;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Меня выгнали из группы {chat.Name}");
    }
}