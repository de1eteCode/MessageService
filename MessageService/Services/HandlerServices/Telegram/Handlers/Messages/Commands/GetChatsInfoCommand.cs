using System.Diagnostics;
using System.Text;
using MessageService.Datas;
using MessageService.Models;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Получение информации о всех чатах, о которых знает бот
/// </summary>
[TelegramUserRole("Системный администратор")]
public class GetChatsInfoCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;
    private readonly TelegramSettings _tgConfiguration;

    public GetChatsInfoCommand(IDatabaseService<DataContext> dbService, IOptionsMonitor<TelegramSettings> optionsMonitor) : base("getchatsinfo", "Получение информации о всех чатах, которые есть в БД") {
        _dbService = dbService;
        _tgConfiguration = optionsMonitor.CurrentValue;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
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

        var stringBuilder = new StringBuilder();

        var tasks = (await allChats.ToListAsync()).Select(chatModel => BuildBlockInfoChat(chatModel, botClient));

        Task.WaitAll(tasks.ToArray());

        foreach (var task in tasks) {
            if (task.IsCompletedSuccessfully) {
                stringBuilder.AppendLine(task.Result);
            }
        }
        stopwatch.Stop();
        await botClient.SendTextMessageAndSplitIfOverfullAsync(message.Chat.Id, stringBuilder.ToString());
        await botClient.SendTextMessageAsync(message.Chat.Id, stopwatch.Elapsed.ToString());
    }

    private Task<string> BuildBlockInfoChat(Datas.Models.Chat chatModel, ITelegramBotClient botClient) {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("ID: " + chatModel.ChatId);
        strBuilder.AppendLine("Имя: " + chatModel.Name);
        if (chatModel.IsJoined) {
            try {
                var chatInfo = botClient.GetChatAsync(chatModel.ChatId!).Result;
                strBuilder.AppendLine("Статус: состою в чате");
            }
            catch (AggregateException ex) when (ex.InnerException!.GetType() == typeof(ApiRequestException) && ((ApiRequestException)ex.InnerException).ErrorCode == 400) { // Not found exception
                strBuilder.AppendLine("Статус: в базе написано что состою, но не состою");
            }
        }
        else {
            strBuilder.AppendLine($"Статус: меня выгнал {chatModel.KickedByUserLogin}, дата {(chatModel.KickedTime != null ? chatModel.KickedTime.Value.ToString("F") : "не найдена")}");
        }
        return Task.FromResult(strBuilder.ToString());
    }
}