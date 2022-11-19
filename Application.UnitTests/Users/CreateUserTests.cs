using Application.Common.Interfaces;
using Application.Users.Commands.CreateUser;

namespace Application.Tests.Users;

public class CreateUserTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();
    }

    [Test]
    public async Task CreateUserCommandHandle() {
        var handler = new CreateUserCommandHandler(_context);
        var request = new CreateUserCommand() {
            Name = "Test user",
            RoleUID = Guid.Parse("508C2BF9-C65E-443C-9D0E-D53A1B745C53"),
            TelegramId = 74745765
        };

        var result = await handler.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(result);
        Assert.That(result.Role.Name, Is.EqualTo("Пользователь"));
        Assert.IsTrue(result.UserGroups.Count == 0);
        Assert.That(result.Name, Is.EqualTo("Test user"));
    }
}