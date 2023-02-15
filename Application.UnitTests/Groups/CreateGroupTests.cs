using Application.Common.Interfaces;
using Application.Groups.Commands.CreateGroup;
using Domain.Entities;

namespace Application.Tests.Groups;

public class CreateGroupTests {
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

        _context.SaveChanges();
    }

    [Test]
    public async Task CreateGroupCommandHandle() {
        var handler = new CreateGroupCommandHandler(_context);
        var request = new CreateGroupCommand() {
            Name = "Test 1",
            OwnerUserUID = Guid.Parse("30bcb63f-4d0b-4618-ad62-a57df0781e15"),
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(result);
        Assert.IsTrue(result.AlternativeId == 1);
        Assert.IsTrue(result.UserGroups.Count == 1);
        Assert.That(result.Name, Is.EqualTo("Test 1"));
    }
}