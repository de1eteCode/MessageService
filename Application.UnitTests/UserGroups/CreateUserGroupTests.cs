using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Tests;
using Application.UserGroups.Commands.CreateUserGroup;
using Domain.Models;

namespace Application.UnitTests.UserGroups;

public class CreateUserGroupTests {
    private IDataContext _context = default!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SetUp]
    public void Setup() {
        _context = TestDatabase.CreateDatabase();

        _context.Groups.Add(new Group() {
            UID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            Name = "Test group 1"
        });

        _context.Users.Add(new User() {
            UID = Guid.Parse("b5570d8b-e736-40f0-be61-f777a75eed93"),
            RoleUID = Guid.Parse("508C2BF9-C65E-443C-9D0E-D53A1B745C53"),
            TelegramId = 3247734,
            Name = "Test user 1"
        });

        _context.SaveChanges();
    }

    [Test]
    public void CreateUserGroupCommandHandle_NotFound() {
        var handle = new CreateUserGroupCommandHandler(_context);
        var request = new CreateUserGroupCommand() {
            GroupUID = Guid.Parse("68982358-11ea-41c4-9232-8096a088a42a"),
            UserUID = Guid.Parse("4568e317-cba1-44e5-b9c3-107618c6af2b")
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await handle.Handle(request, _cancellationTokenSource.Token));
    }

    [Test]
    public async Task CreateUserGroupCommandHandle_Normal() {
        var handle = new CreateUserGroupCommandHandler(_context);
        var request = new CreateUserGroupCommand() {
            GroupUID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            UserUID = Guid.Parse("b5570d8b-e736-40f0-be61-f777a75eed93")
        };

        var result = await handle.Handle(request, _cancellationTokenSource.Token);

        Assert.NotNull(result);
        Assert.That(result.User.Name, Is.EqualTo("Test user 1"));
        Assert.That(result.Group.Name, Is.EqualTo("Test group 1"));
    }

    [Test]
    public async Task CreateUserGroupCommandHandle_Duplicate() {
        var handle = new CreateUserGroupCommandHandler(_context);
        var request = new CreateUserGroupCommand() {
            GroupUID = Guid.Parse("83c91f3b-f96a-4f0b-959d-c629d54587dc"),
            UserUID = Guid.Parse("b5570d8b-e736-40f0-be61-f777a75eed93")
        };

        await handle.Handle(request, _cancellationTokenSource.Token);
        Assert.ThrowsAsync<ExistingEntityException>(async () => await handle.Handle(request, _cancellationTokenSource.Token));
    }
}