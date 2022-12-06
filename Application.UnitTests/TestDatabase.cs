using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests;

//https://stackoverflow.com/questions/56319638/entityframeworkcore-sqlite-in-memory-db-tables-are-not-created
public class TestDatabase : DataContext {

    private TestDatabase(DbContextOptions<DataContext> options) : base(options) {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Userid=postgres;Password=postgres;Database=Test;MaxPoolSize=20;Timeout=15;SslMode=Disable");
    }

    public static TestDatabase CreateDatabase() {
        var db = new TestDatabase(new DbContextOptions<DataContext>());
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Database.Migrate();
        return db;
    }

    public new void Dispose() {
        Database.EnsureDeleted();
        base.Dispose();
    }
}