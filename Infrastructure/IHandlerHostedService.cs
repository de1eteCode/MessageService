using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public interface IHandlerHostedService : IHostedService {
    public bool IsHosted { get; }
}