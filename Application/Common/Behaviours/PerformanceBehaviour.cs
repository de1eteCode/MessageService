using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponce> : IPipelineBehavior<TRequest, TResponce>
    where TRequest : IRequest<TResponce> {
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _stopwatch;

    public PerformanceBehaviour(ILogger<TRequest> logger) {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task<TResponce> Handle(TRequest request, RequestHandlerDelegate<TResponce> next, CancellationToken cancellationToken) {
        _stopwatch.Start();

        var responce = await next();

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds > 500) {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning("Long running request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName, _stopwatch.ElapsedMilliseconds, request);
        }

        return responce;
    }
}