using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.EFCore;
public static class Extensions {

    public static IServiceCollection AddDataRepository(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction = default, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) {
        return services.AddDbContext<DataContext>(optionsAction, contextLifetime, optionsLifetime);

        // Todo: Init repository
    }
}
