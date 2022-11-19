using Application.Chats.Commands.CreateChat;
using Application.Common.Interfaces;

namespace Application.Tests.Chats;

public class CreateChatTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();
    }

    [Test]
    public async Task CeateChatCommandHandle_True() {
        var handler = new CreateChatCommandHandler(_context);
        var request = new CreateChatCommand() {
            Name = "sdsd",
            TelegramChatId = 233543545
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.IsNotNull(result);
        Assert.That(result.UID, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Name, Is.EqualTo(request.Name));
        Assert.That(result.TelegramChatId, Is.EqualTo(request.TelegramChatId));
    }
}