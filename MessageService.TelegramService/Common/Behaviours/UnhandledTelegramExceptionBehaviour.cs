using MediatR;
using MessageService.TelegramService.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace MessageService.TelegramService.Common.Behaviours;

internal class UnhandledTelegramExceptionBehaviour<TRequest, TResponse> : ITelegramPipelineBehavior<TRequest, TResponse>
    where TRequest : ITelegramRequest<TResponse> {
    private readonly ILogger<TRequest> _logger;

    public UnhandledTelegramExceptionBehaviour(ILogger<TRequest> logger) {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        try {
            return await next();
        }
        catch (Exception ex) {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}