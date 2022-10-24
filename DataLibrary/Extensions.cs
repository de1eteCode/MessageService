using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataLibrary.Helpers;

namespace DataLibrary;

public static class Extensions {

    public static IServiceCollection AddDataRepository(this IServiceCollection services, string connString) => services
        .AddDbContext<DataContext>(options => {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(connString);
            options.EnableSensitiveDataLogging();
        })
        .AddTransient<IDatabaseService<DataContext>, ScopeDatabaseService<DataContext>>();
}