using Telegram.BotAPI.GettingUpdates;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramRequestParamsBaseBuilder<TRequest> {

    public void BuildParams(Update update, IEnumerable<string> args, ref TRequest request);
}

internal interface ITelegramRequestParamsBuilder<TRequest> : ITelegramRequestParamsBaseBuilder<TRequest>
    where TRequest : ITelegramRequest {
}

internal interface ITelegramPassiveRequestParamsBuilder<TRequest> : ITelegramRequestParamsBaseBuilder<TRequest>
    where TRequest : ITelegramPassiveRequest {
}