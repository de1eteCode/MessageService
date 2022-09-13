using Microsoft.EntityFrameworkCore;

namespace MessageService.Services.HelperService;

public interface IDatabaseService<T> where T : DbContext {

    public T GetDBContext();
}

public class ScopeDatabaseService<T> : IDatabaseService<T>, IDisposable
    where T : DbContext {
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private IServiceScope _scope = default!;

    public ScopeDatabaseService(IServiceScopeFactory serviceScopeFactory) {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public T GetDBContext() {
        if (_scope == null) {
            _scope = _serviceScopeFactory.CreateScope();
        }

        return _scope.ServiceProvider.GetService<T>() ?? throw new ArgumentNullException();
    }

    public void Dispose() {
        _scope.Dispose();
    }
}