using Application.Common.Interfaces;
using Application.Tests;
using Application.Users.Commands;
using Domain.Models;

namespace Application.UnitTests.Users;

public class UpdateUserTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Users.Add(new User() {
            Name = "Test user 1",
            TelegramId = 83475,
            RoleUID = Guid.Parse("7ADF9DA4-C9B4-4C6D-A75D-2666475BA18E"),
            UID = Guid.Parse("783bfcfa-1e53-45f3-9994-18d822496dcc")
        });

        _context.SaveChanges();
    }

    [Test]
    public async Task UpdateUserCommandHandle() {
        var handler = new UpdateUserCommandHandler(_context);
        var request = new UpdateUserCommand() {
            UserUID = Guid.Parse("783bfcfa-1e53-45f3-9994-18d822496dcc"),
            RoleUID = Guid.Parse("508C2BF9-C65E-443C-9D0E-D53A1B745C53")
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.IsNotNull(result);
        Assert.That(result.Role.Name, Is.EqualTo("Пользователь"));
        Assert.That(result.Role.AlternativeId, Is.EqualTo(3));
    }
}