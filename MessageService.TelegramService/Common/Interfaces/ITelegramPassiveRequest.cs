using MediatR;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramPassiveRequest<out TResponce> : IRequest<TResponce> {
}

internal interface ITelegramPassiveRequest : ITelegramPassiveRequest<Unit> {
}