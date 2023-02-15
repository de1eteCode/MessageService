using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

internal class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull {
    private readonly ILogger<TRequest> _logger;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, IIdentityService identityService, ICurrentUserService currentUserService) {
        _logger = logger;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken) {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? null;
        var userName = string.Empty;

        if (_currentUserService.UserId.GetValueOrDefault() != Guid.Empty) {
            userName = await _identityService.GetUserNameAsync(_currentUserService.UserId!.Value);
        }

        _logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}