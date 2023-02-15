using Application.Common.Interfaces;

namespace TelegramService.Services;

internal class TelegramCurrentUserService : ICurrentUserService {
    public Guid? UserId => throw new NotImplementedException();
}