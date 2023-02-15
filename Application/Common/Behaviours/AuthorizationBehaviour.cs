﻿using Application.Common.Interfaces;
using Application.Common.Security;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours;

internal class AuthorizationBehaviour<TRequest, TResponce> : IPipelineBehavior<TRequest, TResponce>
    where TRequest : IRequest<TResponce> {
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(ICurrentUserService currentUserService, IIdentityService identityService) {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<TResponce> Handle(TRequest request, RequestHandlerDelegate<TResponce> next, CancellationToken cancellationToken) {
        var authorizeAttributes = typeof(TRequest).GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any() == false) {
            return await next();
        }

        if (_currentUserService.UserId == null) {
            throw new UnauthorizedAccessException();
        }

        throw new NotImplementedException();
    }
}