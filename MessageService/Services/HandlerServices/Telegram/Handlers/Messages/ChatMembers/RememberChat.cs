using MessageService.Datas;
using MessageService.Services.HelperService;
using System.Security.AccessControl;
using Telegram.Bot.Types;

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

    internal async Task Execute(Message message) {
        var newMembers = message.NewChatMembers?.ToList() ?? new List<User>();

        if (newMembers.Any()) {
            var me = await _whoIam.GetMeAsync();

            var meInNew = newMembers.FirstOrDefault(e => e.Id == me.Id);

            if (meInNew != null) {
                _logger.LogInformation($"Ура, меня добавили в {message.Chat.Type}");

                if (message.Chat.Type != global::Telegram.Bot.Types.Enums.ChatType.Group) {
                    return;
                }

                var context = _dbService.GetDBContext();

                context.Chats.Add(new Datas.Models.Chat() {
                    ChatId = message.Chat.Id.ToString(),
                    Name = message.Chat.Title
                });

                await context.SaveChangesAsync();
            }
            else {
                _logger.LogInformation($"Были добавлены новые участники: {string.Join(", ", newMembers.Select(e => e.Username))}");
            }
        }
    }
}