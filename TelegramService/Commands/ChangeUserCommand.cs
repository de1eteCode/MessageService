using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Команда изменения пользователя
/// </summary>
internal class ChangeUserCommand : BotCommandAction {
    private readonly IDataContext _context;

    public ChangeUserCommand(IDataContext context) : base("changeuser", "Изменение пользователя") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat!.Id;
        var msgText = message.Text!;

        var roles = await _context.Roles.ToListAsync();

        if (string.IsNullOrWhiteSpace(msgText)) {
            await SendDefaultMessage();
            return;
        }

        var splitedText = msgText.Split(new char[] { ' ' }, 2);

        if (splitedText.Length != 2) {
            await SendDefaultMessage();
            return;
        }

        var idTelegramStr = splitedText.First();

        // проверка на наличие такого пользователя
        var userForChange = await _context.Users.FirstOrDefaultAsync(e => e.TelegramId!.Equals(idTelegramStr));

        if (userForChange == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {idTelegramStr} не найден");
            return;
        }

        var roleId = splitedText.Last();

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = roles.FirstOrDefault(e => e.AlternativeId == roleIdNum);

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(privateChatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            userForChange.Role = selectedRoleUser;
            userForChange.RoleUID = selectedRoleUser.UID;

            _context.Entry(userForChange).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {userForChange.Name} был успешно изменен");
        }
        else {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис изменения пользователя: /changeuser [tg user id] [id роли]\n" +
                "Доступные роли:\n" +
                String.Join("\n", roles.Select(e => String.Join(" - ", e.AlternativeId, e.Name))));
        }
    }
}