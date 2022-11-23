using Application.Groups.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Рассылка сообщения по всем чатам, которые состоят в выбранной пользователем группой
/// </summary>
[UserRole("Системный администратор")]
internal class SendAllChatByGroupCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public SendAllChatByGroupCommand(IMediator mediator) : base("sendallchatbygroup", "Отправка сообщения во все чаты, которые имееются в группе") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msg = message.Text;
        var privateChatId = message.Chat.Id;

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
            var selectedGroup = await _mediator.Send(new GetGroupCommand() { AlternativeId = groupId });

            if (selectedGroup == null) {
                await botClient.SendTextMessageAsync(privateChatId, $"Я не нашел у себя в базе группу с идентификатором {groupId}");
                return;
            }

            // проверка текста для отправки
            var msgToSend = String.Join(" ", splitedText.Skip(1));

            if (msgToSend.Length < 1) {
                await botClient.SendTextMessageAsync(privateChatId, "Может не стоит рассылать пустые сообщения?");
                return;
            }

            // рассылка сообщений
            var chatSended = 0;

            var tasksToSend = selectedGroup.ChatGroups.Where(e => e.IsDeleted == false).Select(e => Task.Run(async () => {
                var sendText = await botClient.SendTextMessageAsync(e.Chat.TelegramChatId, msgToSend!);
                if (sendText != null) {
                    Interlocked.Increment(ref chatSended);
                }
            }));

            if (tasksToSend.Any()) {
                await Task.WhenAll(tasksToSend);

                await botClient.SendTextMessageAsync(privateChatId, $"Сообщение отправлено в {chatSended} чатов");
            }
            else {
                await botClient.SendTextMessageAsync(privateChatId, $"В группе {selectedGroup.Name} нет чатов");
            }
        }
        else {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {groupIdToSend} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис для отправки сообщения во все чаты группы: /sendallchatbygroup [id группы] [текст сообщения]\n" +
                "Доступные группы можно посмотреть командой: /getgroupsinfo");
        }
    }
}