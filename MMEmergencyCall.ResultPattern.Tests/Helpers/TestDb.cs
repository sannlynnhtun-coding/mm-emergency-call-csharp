using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MMEmergencyCall.Databases.AppDbContextModels;

namespace MMEmergencyCall.ResultPattern.Tests.Helpers;

public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestAppDbContext Db { get; }

    public TestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new TestAppDbContext(options);
        Db.Database.EnsureCreated();
    }

    public async Task SaveAsync(params object[] entities)
    {
        Db.AddRange(entities);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
