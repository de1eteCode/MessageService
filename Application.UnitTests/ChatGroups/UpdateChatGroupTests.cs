using Application.ChatGroups.Commands;
using Application.Common.Interfaces;
using Domain.Models;

namespace Application.Tests.ChatGroups;

public class UpdateChatGroupTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Chats.Add(new Chat() {
            UID = Guid.Parse("44462dd1-0308-4289-b07f-9ee9b0ba6ce7"),
            TelegramChatId = 455465,
            Name = "Test chat 1",
            IsJoined = true,
            KickedByUserId = null,
            KickedByUserLogin = null,
            KickedTime = null
        });

        _context.Groups.Add(new Group() {
            UID = Guid.Parse("90cf3cb7-9b5b-436d-b399-5565990fbe87"),
            Name = "Test group 1"
        });

        _context.ChatGroups.Add(new ChatGroup() {
            UID = Guid.Parse("4c608c72-8596-4ec3-b83b-cc11e866b5de"),
            ChatUID = Guid.Parse("44462dd1-0308-4289-b07f-9ee9b0ba6ce7"),
            GroupUID = Guid.Parse("90cf3cb7-9b5b-436d-b399-5565990fbe87"),
            IsDeleted = false
        });

        _context.SaveChanges();
    }

    [Test]
    public async Task UpdateChatGroupCommandHandle() {
        var handler = new UpdateChatGroupCommandHandler(_context);
        var request = new UpdateChatGroupCommand() {
            ChatGroupUID = Guid.Parse("4c608c72-8596-4ec3-b83b-cc11e866b5de"),
            IsDeleted = true
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(result);
        Assert.IsTrue(result.IsDeleted);
        Assert.That(result.Chat.UID, Is.EqualTo(Guid.Parse("44462dd1-0308-4289-b07f-9ee9b0ba6ce7")));
        Assert.That(result.Group.UID, Is.EqualTo(Guid.Parse("90cf3cb7-9b5b-436d-b399-5565990fbe87")));
    }
}