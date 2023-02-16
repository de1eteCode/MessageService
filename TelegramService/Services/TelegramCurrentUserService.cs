using Application.Common.Interfaces;
using TelegramService.Interfaces;

namespace TelegramService.Services;

internal class TelegramCurrentUserService : ICurrentUserService {
    private readonly ICurrentTelegramUpdate _update;
    private readonly IDataContext _context;

    public TelegramCurrentUserService(ICurrentTelegramUpdate update, IDataContext context) {
        _update = update;
        _context = context;
    }

    public Guid? UserId => _context.Users.FirstOrDefault(e => e.TelegramId.Equals(_update.GetFromUserId() ?? -1))?.UID ?? null;
}