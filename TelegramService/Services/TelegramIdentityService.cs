using Application.Common.Interfaces;

namespace TelegramService.Services;

internal class TelegramIdentityService : IIdentityService {

    public Task<bool> AuthorizeAsync(Guid userId) {
        throw new NotImplementedException();
    }

    public Task<object> CreateUserAsync() {
        throw new NotImplementedException();
    }

    public Task<string> GetUserNameAsync(Guid userId) {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(Guid userId, string roleName) {
        throw new NotImplementedException();
    }
}