using Application.Common.Interfaces;

namespace Infrastructure.Services;

internal class CurrentUserService : ICurrentUserService {
    public Guid? UserId => throw new NotImplementedException();
}