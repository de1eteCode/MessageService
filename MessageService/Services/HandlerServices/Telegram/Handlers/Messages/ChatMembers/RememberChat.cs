using MessageService.Datas;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.ChatMembers;

/// <summary>
/// Обработчик для чата, который смотрит на то, кто присоединился.
/// </summary>
public class RememberChat {
    private readonly IDatabaseService<DataContext> _dbService;
    private readonly IWhoIam _whoIam;
    private readonly ILogger<RememberChat> _logger;

    public RememberChat(IWhoIam whoIam, IDatabaseService<DataContext> dbService, ILogger<RememberChat> logger) {
        _dbService = dbService;
        _whoIam = whoIam;
        _logger = logger;
    }

    public async Task ExecuteActionAsync(Message message) {
        if (message.Chat.Type != ChatType.Group) {
            return;
        }

        var newMembers = message.NewChatMembers?.ToList() ?? new List<User>();

        if (newMembers.Any()) {
            var me = await _whoIam.GetMeAsync();

            var meInNew = newMembers.FirstOrDefault(e => e.Id == me.Id);

            if (meInNew != null) {
                var context = _dbService.GetDBContext();

                // проверка на ранее добавления бота в чат
                var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId!.Equals(message.Chat.Id.ToString()));

                if (chat != null) {
                    // бот ранее состоял в этом чате
                    chat.IsJoined = true;
                    chat.Name = message.Chat.Title;
                    chat.KickedTime = null;
                    chat.KickedByUserLogin = null;

                    context.Entry(chat).State = EntityState.Modified;
                }
                else {
                    // бот в первые в этом чате
                    chat = new Datas.Models.Chat() {
                        ChatId = message.Chat.Id.ToString(),
                        Name = message.Chat.Title
                    };

                    context.Entry(chat).State = EntityState.Added;
                }

                await context.SaveChangesAsync();
            }
            else {
                _logger.LogInformation($"Были добавлены новые участники: {string.Join(", ", newMembers.Select(e => e.Username))}");
            }
        }
    }
}