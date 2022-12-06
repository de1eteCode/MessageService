using Application.Chats.Commands;
using Application.Common.Interfaces;
using Domain.Models;

namespace Application.Tests.Chats;

public class UpdateChatTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private Chat _chat = new Chat() {
        UID = Guid.NewGuid(),
        Name = "Test",
        TelegramChatId = 203302,
        IsJoined = false,
        KickedByUserId = 11011,
        KickedByUserLogin = "test_user"
    };

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Chats.Add(_chat);

        _context.SaveChanges();
    }

    [Test]
    public async Task UpdateChatCommandHandle_True() {
        var handler = new UpdateChatCommandHandler(_context);
        var request = new UpdateChatCommand() {
            UID = _chat.UID,
            Name = "Test1",
            TelegramChatId = _chat.TelegramChatId,
            IsJoined = true,
            KickedUserId = null,
            KickedUserLogin = null
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.IsNotNull(result);
        Assert.That(result.UID, Is.EqualTo(_chat.UID));
        Assert.That(result.Name, Is.EqualTo("Test1"));
        Assert.That(result.IsJoined, Is.EqualTo(true));
        Assert.That(result.KickedByUserId, Is.Null);
        Assert.That(result.KickedByUserLogin, Is.Null);
    }
}