using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Рассылка сообщения по всем чатам, которые состоят в выбранной пользователем группой
/// </summary>
[UserRole("Системный администратор")]
internal class SendAllChatByGroupCommand : BotCommandAction {
    private readonly IDataContext _context;

    public SendAllChatByGroupCommand(IDataContext context) : base("sendallchatbygroup", "Отправка сообщения во все чаты, которые имееются в группе") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msg = message.Text;
        var chatId = message.Chat.Id;

        var availableGroups = await _context.Groups.ToListAsync();

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage();
            return;
        }

        var splitedText = msg.Split(' ');

        if (splitedText.Length < 2) {
            await SendDefaultMessage();
            return;
        }

        var groupIdToSend = splitedText.First();

        if (int.TryParse(groupIdToSend, out var groupId)) {
            // проверка выбранной группы
            var selectedGroup = availableGroups.FirstOrDefault(e => e.AlternativeId == groupId);

            if (selectedGroup == null) {
                await botClient.SendTextMessageAsync(chatId, $"Я не нашел у себя в базе группу с идентификатором {groupId}");
                return;
            }

            // проверка текста для отправки
            var msgToSend = String.Join(" ", splitedText.Skip(1));

            if (msgToSend.Length < 1) {
                await botClient.SendTextMessageAsync(chatId, "Может не стоит рассылать пустые сообщения?");
                return;
            }

            // рассылка сообщений
            var chatIds = _context.ChatGroups.Where(e => e.Group.AlternativeId == groupId && e.IsDeleted == false).Select(e => e.Chat.TelegramChatId!);

            var chatSended = 0;

            if (chatIds.Any()) {
                await chatIds.ForEachAsync(chatId => {
                    var msg = botClient.SendTextMessageAsync(chatId, msgToSend!);
                    if (msg != null) {
                        Interlocked.Increment(ref chatSended);
                    }
                });
                await botClient.SendTextMessageAsync(chatId, $"Сообщение отправлено в {chatSended} чатов");
            }
            else {
                await botClient.SendTextMessageAsync(chatId, $"В группе {selectedGroup.Name} нет чатов");
            }
        }
        else {
            await botClient.SendTextMessageAsync(chatId, $"Хм, я думаю {groupIdToSend} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(chatId,
                "Синтаксис для отправки сообщения во все чаты группы: /sendallchatbygroup [id группы] [текст сообщения]\n" +
                "Доступные группы можно посмотреть командой: /getgroupsinfo");
        }
    }
}