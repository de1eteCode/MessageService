using MediatR;
using MessageService.TelegramService.Common.Abstracts;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Domain.Models;
using Telegram.BotAPI.AvailableMethods;
using Application.Groups.Queries;
using MessageService.TelegramService.Common.Attributes;

namespace MessageService.TelegramService.Commands;

[TelegramUserRole("Системный администратор")]
internal record SendAllChatByGroupCommand : ITelegramRequest {
    public BotCommand BotCommand => new BotCommand("sendallchatbygroup", "Отправка сообщения во все чаты, которые имееются в группе");

    /// <summary>
    /// Идентификатор чата с пользователем, который прислал сообщение
    /// </summary>
    public long PrivateChatId { get; set; }

    /// <summary>
    /// Текст сообщения для отправки
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Альтернативный идентификатор <see cref="Group"/>
    /// </summary>
    public int? GroupAlternativeId { get; set; }
}

internal class SendAllChatByGroupCommandHandler : TelegramRequestHandler<SendAllChatByGroupCommand> {
    private readonly IMediator _mediator;

    public SendAllChatByGroupCommandHandler(BotClient botClient, IMediator mediator) : base(botClient) {
        _mediator = mediator;
    }

    public override async Task<Unit> Handle(SendAllChatByGroupCommand request, BotClient botClient, CancellationToken cancellationToken) {
        if (request.GroupAlternativeId == null || string.IsNullOrEmpty(request.Message)) {
            return await SendDefaultMessage(request.PrivateChatId, cancellationToken);
        }

        var group = await _mediator.Send(new GetGroupCommand() { AlternativeId = (int)request.GroupAlternativeId });

        if (group == null) {
            await botClient.SendMessageAsync(request.PrivateChatId, "", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var chats = group.ChatGroups.Where(e => e.IsDeleted == false).Select(e => e.Chat).ToList();

        if (chats.Any() == false) {
            await botClient.SendMessageAsync(request.PrivateChatId, $"В группе {group.Name} нет чатов", cancellationToken: cancellationToken);
            return Unit.Value;
        }

        var chatSended = 0;

        var tasksToSend = chats.Select(chat => Task.Run(async () => {
            try {
                var res = await botClient.SendMessageAsync(chat.TelegramChatId, request.Message, cancellationToken: cancellationToken);

                if (res != null) {
                    Interlocked.Increment(ref chatSended);
                }
            }
            catch (Exception) {
                // Todo: Обработка ошибки, если бота кикнули из чата, когда был оффлайн
            }
        }));

        await Task.WhenAll(tasksToSend);

        await botClient.SendMessageAsync(request.PrivateChatId, $"Сообщение отправлено в {chatSended} чатов", cancellationToken: cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> SendDefaultMessage(long privateChatId, CancellationToken cancellationToken) {
        await _botClient.SendMessageAsync(
            privateChatId,
            "Синтаксис для отправки сообщения во все чаты группы: /sendallchatbygroup [id группы] [текст сообщения]",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

internal class SendAllChatByGroupCommandParamsBuilder : ITelegramRequestParamsBuilder<SendAllChatByGroupCommand> {

    public void BuildParams(Update update, IEnumerable<string> args, ref SendAllChatByGroupCommand request) {
        request.PrivateChatId = update.Message.Chat.Id;

        if (args.Count() < 2) {
            return;
        }

        if (int.TryParse(args.ElementAt(0), out int val)) {
            request.GroupAlternativeId = val;
        }

        request.Message = string.Join(" ", args.Skip(1));
    }
}