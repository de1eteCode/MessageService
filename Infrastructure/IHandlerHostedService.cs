using Infrastructure.Enums;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Интерфейс сервиса для хостинга
/// </summary>
public interface IHandlerHostedService : IHostedService {

    /// <summary>
    /// Состояние сервиса
    /// </summary>
    public ServiceState State { get; }
}