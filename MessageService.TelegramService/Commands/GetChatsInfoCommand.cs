using Application.Chats.Queries;
using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Attributes;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Commands;

[TelegramUserRole("Системный администратор")]
internal record GetChatsInfoCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("getchatsinfo", "Получение информации о всех чатах, которые есть в БД");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Необязательный параметр. При значении True, выводит весь список чатов, включая те, из которых выгнали бота
    /// </summary>
    public bool All { get; set; }
}

internal class GetChatsInfoCommandHandler : TelegramRequestHandler<GetChatsInfoCommand> {
    private readonly IMediator _mediator;

    public GetChatsInfoCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(GetChatsInfoCommand request, BotClient botClient, CancellationToken cancellationToken) {
        var chats = await _mediator.Send(new GetChatsCommand() {
            Predicate = request.All ? null : chat => chat.IsJoined == true
        });

        var countChats = chats.Count();

        await botClient.SendMessageAsync(request.PrivateChatId,
            $"Вот что я знаю о своих чатах, их всего {countChats}. {(countChats > 5 ? "Готовтесь к спаму" : "")}",
            cancellationToken: cancellationToken);

        var stringBuilder = new StringBuilder();

        var tasks = chats.Select(chatModel => BuildBlockInfoChat(chatModel));

        await Task.WhenAll(tasks.ToArray());

        foreach (var task in tasks) {
            if (task.IsCompletedSuccessfully) {
                stringBuilder.AppendLine(task.Result);
            }
        }

        await botClient.SendMessageAndSplitIfOverfullAsync(request.PrivateChatId, stringBuilder.ToString(), cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private Task<string> BuildBlockInfoChat(Domain.Models.Chat chatModel) {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("ID: " + chatModel.TelegramChatId);
        strBuilder.AppendLine("Имя: " + chatModel.Name);
        if (chatModel.IsJoined) {
            try {
                var chatInfo = _botClient.GetChatAsync(chatModel.TelegramChatId!).Result;
                strBuilder.AppendLine("Статус: состою в чате");
            }
            catch (AggregateException ex) when (ex.InnerException!.GetType() == typeof(BotRequestException) && ((BotRequestException)ex.InnerException).ErrorCode == 400) { // Not found exception
                strBuilder.AppendLine("Статус: в базе написано что состою, но не состою");
            }
        }
        else {
            strBuilder.AppendLine($"Статус: меня выгнал {chatModel.KickedByUserLogin}, дата {(chatModel.KickedTime != null ? chatModel.KickedTime.Value.ToString("F") : "не найдена")}");
        }
        return Task.FromResult(strBuilder.ToString());
    }
}

internal class GetChatsInfoCommandParamsBuilder : ITelegramRequestParamsBuilder<GetChatsInfoCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref GetChatsInfoCommand request) {
        request.PrivateChatId = update.Message!.Chat!.Id;
        request.All = args.Any() && args.First().ToLower().Equals("all");
    }
}