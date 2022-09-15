using System.Text;
using MessageService.Datas;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Получение информации о всех чатах, о которых знает бот
/// </summary>
[TelegramUserRole("Системный администратор")]
public class GetChatsInfoCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public GetChatsInfoCommand(IDatabaseService<DataContext> dbService) : base("getchatsinfo", "Получение информации о всех чатах, которые есть в БД") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var context = _dbService.GetDBContext();

        IQueryable<Datas.Models.Chat> allChats = context.Chats;

        var msg = message.Text!;
        if (string.IsNullOrEmpty(msg)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Есть возможность посмотреть все чаты: /getchatsinfo all");
        }

        if (msg.Equals("all") == false) {
            allChats = allChats.Where(e => e.IsJoined);
        }

        if (allChats.Any() == false) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Сейчас нет чатов, в которых я состаю");
            return;
        }

        var countChats = allChats.Count();
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вот что я знаю о своих чатах, их всего {countChats}. {(countChats > 5 ? "Готовтесь к спаму с:" : "")}");

        await allChats.ForEachAsync(chatModel => {
            var threadSB = new StringBuilder();
            var chatInfo = botClient.GetChatAsync(chatModel.ChatId!).Result;
            threadSB.AppendLine("ID: " + chatInfo.Id);
            threadSB.AppendLine("Имя: " + chatInfo.Title);
            threadSB.AppendLine("Статус: " + (chatModel.IsJoined ? "состою в чате" : $"меня выгнал {chatModel.KickedByUserLogin}, дата {(chatModel.KickedTime != null ? chatModel.KickedTime.Value.ToString("F") : "не найдена")}"));
            //if (chatInfo != null) {
            //    var myPermis = chatInfo.Permissions;
            //    if (myPermis != null) {
            //        threadSB.AppendLine("Мои привелегии в чате:");
            //        threadSB.AppendLine("Могу отправлять сообщения: " + GetRuYesORNo(myPermis.CanSendMessages ?? false));
            //        threadSB.AppendLine("Могу отправлять медиа сообщения: " + GetRuYesORNo(myPermis.CanSendMediaMessages ?? false));
            //        threadSB.AppendLine("Могу отправлять другие сообщения: " + GetRuYesORNo(myPermis.CanSendOtherMessages ?? false));
            //        threadSB.AppendLine("Могу изменять информацию: " + GetRuYesORNo(myPermis.CanChangeInfo ?? false));
            //        threadSB.AppendLine("Могу приглашать: " + GetRuYesORNo(myPermis.CanInviteUsers ?? false));
            //        threadSB.AppendLine("Могу закреплять сообщения: " + GetRuYesORNo(myPermis.CanPinMessages ?? false));
            //    }
            //}
            //else {
            //    threadSB.AppendLine($"О чате {chatModel.Name} ({chatModel.ChatId!}) не нашел информацию. \n");
            //}

            botClient.SendTextMessageAsync(message.Chat.Id, threadSB.ToString());
        });
    }

    private string GetRuYesORNo(bool status) {
        return status ? "Да" : "Нет";
    }
}