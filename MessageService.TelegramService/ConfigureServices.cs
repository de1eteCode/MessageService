using MediatR;
using MessageService.TelegramService.Common.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices {

    public static IServiceCollection AddTelegramService(this IServiceCollection services) {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledTelegramExceptionBehaviour<,>));

        return services;
    }
}