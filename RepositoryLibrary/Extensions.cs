using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Helpers;

namespace RepositoryLibrary;

public static class Extensions {

    public static IServiceCollection AddDataRepository(this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = default, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) => services
        .AddDbContext<DataContext>(optionsAction, contextLifetime, optionsLifetime)
        .AddTransient<IDatabaseService<DataContext>, ScopeDatabaseService<DataContext>>();
}