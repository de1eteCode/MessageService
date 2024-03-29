﻿using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TelegramService.Attributes;
using TelegramService.Extensions;

namespace TelegramService.Commands;

/// <summary>
/// Получение информации о всех чатах, о которых знает бот
/// </summary>
[UserRole("Системный администратор")]
internal class GetChatsInfoCommand : BotCommandAction {
    private readonly IDataContext _context;

    public GetChatsInfoCommand(IDataContext context) : base("getchatsinfo", "Получение информации о всех чатах, которые есть в БД") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        IQueryable<Domain.Models.Chat> allChats = _context.Chats;

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

        await botClient.SendTextMessageAndSplitIfOverfullAsync(message.Chat.Id, stringBuilder.ToString());
    }

    private Task<string> BuildBlockInfoChat(Domain.Models.Chat chatModel, ITelegramBotClient botClient) {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("ID: " + chatModel.TelegramChatId);
        strBuilder.AppendLine("Имя: " + chatModel.Name);
        if (chatModel.IsJoined) {
            try {
                var chatInfo = botClient.GetChatAsync(chatModel.TelegramChatId!).Result;
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