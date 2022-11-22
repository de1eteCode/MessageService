using MediatR;
using MessageService.TelegramService.Common.Interfaces;
using Telegram.BotAPI;

namespace MessageService.TelegramService.Common.Abstracts;

internal abstract class TelegramPassiveRequestHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : ITelegramPassiveRequest {
    protected readonly BotClient _botClient;

    protected TelegramPassiveRequestHandler(BotClient botClient) {
        _botClient = botClient;
    }

    public Task<Unit> Handle(TRequest request, CancellationToken cancellationToken) {
        return Handle(request, _botClient, cancellationToken);
    }

    public abstract Task<Unit> Handle(TRequest request, BotClient botClient, CancellationToken cancellationToken);
}