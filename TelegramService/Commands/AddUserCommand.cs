using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Добавление нового пользователя в БД <see cref="DataContext"/>
/// </summary>
[UserRole("Системный администратор")]
[Login("de1alex")]
internal class AddUserCommand : BotCommandAction {
    private readonly IDataContext _context;

    public AddUserCommand(IDataContext context) : base("adduser", "Добавление пользователя") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgText = message.Text!;
        var chatId = message.Chat.Id;

        var roles = await _context.Roles.ToListAsync();

        if (string.IsNullOrEmpty(msgText)) {
            await SendDefaultMsg();
            return;
        }

        var splitedText = msgText.Split(' ');

        if (splitedText.Length != 3) {
            await SendDefaultMsg();
            return;
        }

        var idTelegramStr = splitedText[(int)PositionArgs.TelegramId];
        if (long.TryParse(idTelegramStr, out long idTelegram) == false) {
            await botClient.SendTextMessageAsync(chatId, $"{idTelegramStr} не похож на идентификатор пользователя Telegram");
            return;
        }

        // проверка на наличие такого пользователя
        var addedUser = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(idTelegram));

        if (addedUser != null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {addedUser.Name} был ранее добавлен");
            return;
        }

        var roleId = splitedText[(int)PositionArgs.RoleId];

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = roles.FirstOrDefault(e => e.AlternativeId == roleIdNum);

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(chatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            var newUser = new Domain.Models.User() {
                TelegramId = idTelegram,
                Name = String.Join(" ", splitedText.Skip((int)PositionArgs.Name)),
                Role = selectedRoleUser,
                RoleUID = selectedRoleUser.UID
            };

            _context.Entry(newUser).State = EntityState.Added;

            await _context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(chatId, $"Пользователь {newUser.Name} был успешно добавлен с ролью {newUser.Role.Name}");
        }
        else {
            await botClient.SendTextMessageAsync(chatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMsg() {
            return botClient.SendTextMessageAsync(chatId,
                "Синтаксис для добавления пользователя: /adduser [id роль] [id telegram] [Имя]\n" +
                "Доступные роли:\n" +
                String.Join("\n", roles.OrderBy(e => e.AlternativeId).Select(e => String.Join(" - ", e.AlternativeId, e.Name))));
        }
    }

    private enum PositionArgs : int {
        RoleId = 0,
        TelegramId = 1,
        Name = 2
    }
}