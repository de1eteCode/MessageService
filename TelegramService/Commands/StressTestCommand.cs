using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

[UserRole("Системный администратор")]
internal class StressTestCommand : BotCommandAction {
    private readonly ILogger<StressTestCommand> _logger;

    public StressTestCommand(ILogger<StressTestCommand> logger) : base("stresstest", "dbg command") {
        _logger = logger;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat.Id;
        var msg = message.Text!;

        if (string.IsNullOrEmpty(msg)) {
            await SendDefaultMessage(botClient, privateChatId);
            return;
        }

        if (int.TryParse(msg, out int count) == false) {
            await SendDefaultMessage(botClient, privateChatId);
            return;
        }

        int complited = 0;
        var watch = new Stopwatch();
        var previousTime = TimeSpan.FromSeconds(0);
        watch.Start();
        while (count > complited) {
            await botClient.GetMeAsync();
            complited++;

            if (complited % 10 == 0) {
                await botClient.SendTextMessageAsync(privateChatId, $"Complited: {complited} | Time: {watch.Elapsed} ({(TimeSpan.FromTicks(watch.ElapsedTicks - previousTime.Ticks))})");
                previousTime = watch.Elapsed;
            }
        }
        watch.Stop();
        if (complited < 10) {
            await botClient.SendTextMessageAsync(privateChatId, $"Total complited: {complited} | Toral time: {watch.Elapsed.ToString()}");
        }
    }

    private Task SendDefaultMessage(ITelegramBotClient botClient, long chatId) {
        return botClient.SendTextMessageAsync(chatId,
            "Синтаксис стресс теста: /stresstest [count]\n" +
            $"Где count: 1 - {int.MaxValue}");
    }
}