using Infrastructure.Enums;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public interface IHandlerHostedService : IHostedService {
    public ServiceState State { get; }
}