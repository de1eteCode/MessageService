using MessageService.Datas;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services.HelperService;

public interface IDatabaseService<T> where T : DbContext {

    public T GetDBContext();
}

public class ScopeDatabaseService : IDatabaseService<DataContext>, IDisposable {
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private IServiceScope _scope = default!;

    public ScopeDatabaseService(IServiceScopeFactory serviceScopeFactory) {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public DataContext GetDBContext() {
        if (_scope == null) {
            _scope = _serviceScopeFactory.CreateScope();
        }

        return _scope.ServiceProvider.GetService<DataContext>() ?? throw new ArgumentNullException();
    }

    public void Dispose() {
        _scope.Dispose();
    }
}