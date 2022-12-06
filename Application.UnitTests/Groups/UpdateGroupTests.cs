using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Groups.Commands;
using Domain.Models;

namespace Application.Tests.Groups;

public class UpdateGroupTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Users.Add(new User() {
            RoleUID = Guid.Parse("7ADF9DA4-C9B4-4C6D-A75D-2666475BA18E"),
            Name = "Test user",
            TelegramId = 83457475,
            UID = Guid.Parse("30bcb63f-4d0b-4618-ad62-a57df0781e15")
        });

        _context.Groups.Add(new Group() {
            UID = Guid.Parse("b29dabe4-0911-4ece-8736-aa1007aa5015"),
            AlternativeId = 5,
            Name = "Test group"
        });

        _context.UserGroups.Add(new UserGroup() {
            UID = Guid.Parse("52074ffb-a911-4aee-8d24-2f2c318f6d36"),
            GroupUID = Guid.Parse("b29dabe4-0911-4ece-8736-aa1007aa5015"),
            UserUID = Guid.Parse("30bcb63f-4d0b-4618-ad62-a57df0781e15")
        });

        _context.SaveChanges();
    }

    [Test]
    public void UpdateGroupCommandHandle_NotFound() {
        var handler = new UpdateGroupCommandHandler(_context);
        var request = new UpdateGroupCommand() {
            GroupUID = Guid.Parse("ffe9553c-2e4d-4024-b9b4-1d2f1458d06f"),
            Name = "Change name group"
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, _cancellationTokenSource.Token));
    }

    [Test]
    public async Task UpdateGroupCommandHandle() {
        var handler = new UpdateGroupCommandHandler(_context);
        var request = new UpdateGroupCommand() {
            GroupUID = Guid.Parse("b29dabe4-0911-4ece-8736-aa1007aa5015"),
            Name = "Change name group"
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(result);
        Assert.IsTrue(result.AlternativeId == 5);
        Assert.IsTrue(result.UserGroups.Count == 1);
        Assert.That(result.Name, Is.EqualTo("Change name group"));
    }
}