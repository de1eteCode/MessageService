using MessageService.Datas;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Команда изменения пользователя
/// </summary>
public class ChangeUserCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public ChangeUserCommand(IDatabaseService<DataContext> dbService) : base("changeuser", "Изменение пользователя") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var privateChatId = message.Chat!.Id;
        var msgText = message.Text!;

        var context = _dbService.GetDBContext();
        var roles = await context.Roles.ToListAsync();

        if (string.IsNullOrWhiteSpace(msgText)) {
            await SendDefaultMessage();
            return;
        }

        var splitedText = msgText.Split(new char[] { ' ' }, 2);

        if (splitedText.Length != 2) {
            await SendDefaultMessage();
            return;
        }

        var tgUserName = splitedText.First();

        // проверка на наличие такого пользователя
        var userForChange = await context.Users.FirstOrDefaultAsync(e => e.Name!.Equals(tgUserName));

        if (userForChange == null) {
            await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {tgUserName} не найден");
            return;
        }

        var roleId = splitedText.Last();

        if (int.TryParse(roleId, out int roleIdNum)) {
            var selectedRoleUser = roles.FirstOrDefault(e => e.RoleId == roleIdNum);

            if (selectedRoleUser == null) {
                await botClient.SendTextMessageAsync(privateChatId, "Я не нашел роль под id " + roleIdNum);
                return;
            }

            userForChange.Role = selectedRoleUser;
            userForChange.RoleId = roleIdNum;

            context.Entry(userForChange).State = EntityState.Modified;

            await context.SaveChangesAsync();

            await botClient.SendTextMessageAsync(privateChatId, $"Пользователь {userForChange.Name} был успешно изменен");
        }
        else {
            await botClient.SendTextMessageAsync(privateChatId, $"Хм, я думаю {roleId} не похож на те идентификаторы, что я отправил");
        }

        return;

        Task SendDefaultMessage() {
            return botClient.SendTextMessageAsync(privateChatId,
                "Синтаксис изменения пользователя: /changeuser [tg username] [id роли]\n" +
                "Доступные роли:\n" +
                String.Join("\n", roles.Select(e => String.Join(" - ", e.RoleId, e.RoleName))));
        }
    }
}