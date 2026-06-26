using Microsoft.EntityFrameworkCore;
using MMEmergencyCall.Databases.AppDbContextModels;

namespace MMEmergencyCall.ResultPattern.Tests.Helpers;

public class TestAppDbContext : AppDbContext
{
    public TestAppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmergencyRequest>()
            .Property(x => x.RequestTime)
            .Metadata.SetDefaultValueSql(null);

        modelBuilder.Entity<EmergencyService>()
            .Property(x => x.ServiceStatus)
            .Metadata.SetDefaultValueSql(null);

        modelBuilder.Entity<User>()
            .Property(x => x.UserStatus)
            .Metadata.SetDefaultValueSql(null);
    }
}
