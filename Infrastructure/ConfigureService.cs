using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices {

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Infrastructure: EF Core
        services.AddDbContext<IDataContext, DataContext>(options => {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(configuration.GetConnectionString("psql"));

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }
}