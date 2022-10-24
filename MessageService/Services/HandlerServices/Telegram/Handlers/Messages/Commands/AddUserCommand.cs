using MessageService.Services.HandlerServices.Telegram.Attributes;
using DataLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Добавление нового пользователя в БД <see cref="DataContext"/>
/// </summary>
[TelegramUserRole("Системный администратор")]
[TelegramLogin("de1alex")]
public class AddUserCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public AddUserCommand(IDatabaseService<DataContext> dbService) : base("adduser", "Добавление пользователя") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgText = message.Text!;
        var chatId = message.Chat.Id;

        var context = _dbService.GetDBContext();
        var roles = await context.Roles.ToListAsync();

        if (string.IsNullOrEmpty(msgText)) {
            await SendDefaultMsg();
            return;
        }

        var splitedText = msgText.Split(new char[] { ' ' });

        if (splitedText.Length < 3) {
            await SendDefaultMsg();
            return;
        }

        var idTelegramStr = splitedText[(int)PositionArgs.TelegramId];
        long idTelegram;
        if (long.TryParse(idTelegramStr, out idTelegram) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{idTelegramStr} не похож на идентификатор пользователя Telegram");
            return;
        }

        // проверка на наличие такого пользователя
        var addedUser = await context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(idTelegram));

        if (addedUser != null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {addedUser.Name} был ранее добавлен");
            return;
        }

        var roleId = splitedText[(int)PositionArgs.RoleId];

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = roles.FirstOrDefault(e => e.RoleId == roleIdNum);

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(chatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            var newUser = new DataLibrary.Models.User() {
                TelegramId = idTelegram,
                Name = String.Join(" ", splitedText.Skip((int)PositionArgs.Name)),
                Role = selectedRoleUser,
                RoleId = selectedRoleUser.RoleId
            };

            context.Entry(newUser).State = EntityState.Added;

            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(chatId, $"Пользователь {newUser.Name} был успешно добавлен с ролью {newUser.Role.RoleName}");
        }
        else {
            await botClient.SendTextMessageAsync(chatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMsg() {
            return botClient.SendTextMessageAsync(chatId,
                "Синтаксис для добавления пользователя: /adduser [id роль] [id telegram] [Имя]\n" +
                "Доступные роли:\n" +
                String.Join("\n", roles.Select(e => String.Join(" - ", e.RoleId, e.RoleName))));
        }
    }

    private enum PositionArgs : int {
        RoleId = 0,
        TelegramId = 1,
        Name = 2
    }
}