using MessageService.Datas;
using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
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

        var splitedText = msgText.Split(new char[] { ' ' }, 2);

        if (splitedText.Length != 2) {
            await SendDefaultMsg();
            return;
        }

        var tgUserName = splitedText.First();

        // проверка на наличие такого пользователя
        var addedUser = await context.Users.FirstOrDefaultAsync(e => e.Name!.Equals(tgUserName));

        if (addedUser != null) {
            await botClient.SendTextMessageAsync(chatId, $"Пользователь {addedUser.Name} был ранее добавлен");
            return;
        }

        var roleId = splitedText.Last();

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = roles.FirstOrDefault(e => e.RoleId == roleIdNum);

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(chatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            var newUser = new Datas.Models.User() {
                Id = "@" + tgUserName,
                Name = tgUserName,
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
                "Синтаксис для добавления пользователя: /adduser [Логин в телеграм] [Роль]\n" +
                "Доступные роли:\n" +
                String.Join("\n", roles.Select(e => String.Join(" - ", e.RoleId, e.RoleName))));
        }
    }
}