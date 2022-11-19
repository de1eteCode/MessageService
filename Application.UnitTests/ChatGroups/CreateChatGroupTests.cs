using Application.ChatGroups.Commands.CreateChatGroup;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Models;

namespace Application.Tests.ChatGroups;

public class CreateChatGroupTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Chats.Add(new Chat() {
            UID = Guid.Parse("481c0465-21f2-48b6-8d14-cceaa7e8e786"),
            TelegramChatId = 455465,
            Name = "Test chat 1",
            IsJoined = true,
            KickedByUserId = null,
            KickedByUserLogin = null,
            KickedTime = null
        });

        _context.Groups.Add(new Group() {
            UID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            Name = "Test group 1"
        });

        _context.SaveChanges();
    }

    [Test]
    public void CreateChatGroupCommandHandle_NotFound() {
        var handler = new CreateChatGroupCommandHandler(_context);
        var request = new CreateChatGroupCommand() {
            GroupUID = Guid.NewGuid(),
            ChatUID = Guid.NewGuid(),
            IsDeleted = false
        };

        Assert.ThrowsAsync<NotFoundException>(async () =>
            await handler.Handle(request, _cancellationTokenSource.Token));
    }

    [Test]
    public async Task CreateChatGroupCommandHandle_Normal() {
        var handler = new CreateChatGroupCommandHandler(_context);
        var request = new CreateChatGroupCommand() {
            GroupUID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            ChatUID = Guid.Parse("481c0465-21f2-48b6-8d14-cceaa7e8e786"),
            IsDeleted = false
        };

        var chatGroup = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(chatGroup);
        Assert.That(chatGroup.ChatUID, Is.EqualTo(Guid.Parse("481c0465-21f2-48b6-8d14-cceaa7e8e786")));
        Assert.That(chatGroup.GroupUID, Is.EqualTo(Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc")));
        Assert.That(chatGroup.IsDeleted, Is.EqualTo(false));
        Assert.That(chatGroup.Chat.Name, Is.EqualTo("Test chat 1"));
        Assert.That(chatGroup.Group.Name, Is.EqualTo("Test group 1"));
    }

    [Test]
    public async Task CreateChatGroupCommandHandle_Dupplicate() {
        var handler = new CreateChatGroupCommandHandler(_context);
        var request = new CreateChatGroupCommand() {
            GroupUID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            ChatUID = Guid.Parse("481c0465-21f2-48b6-8d14-cceaa7e8e786"),
            IsDeleted = false
        };

        var chatGroup = await handler.Handle(request, _cancellationTokenSource.Token);
        Assert.ThrowsAsync<ExistingEntityException>(async () => await handler.Handle(request, _cancellationTokenSource.Token));
    }
}