using MMEmergencyCall.Domain.Admin.Features.Dashboard;
using MMEmergencyCall.Domain.Admin.Features.TotalActive;
using MMEmergencyCall.Domain.Admin.Features.TownshipTopTenRequest;
using MMEmergencyCall.Domain.Admin.Features.TownshipTopTenService;
using MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;

namespace MMEmergencyCall.ResultPattern.Tests;

public class AdminDashboardServiceTests
{
    [Fact]
    public async Task Dashboard_returns_reader_data_and_system_error()
    {
        var model = new DashboardModel
        {
            RequestSummary = new RequestSummaryModel { DailyRequest = 1, WeeklyRequest = 2, MonthlyRequest = 3 },
            TopTenServicePerTownship = new(),
            ServiceProviderActivity = new(),
            TopTenRequestPerUser = new()
        };
        var service = new DashboardService(Logger.For<DashboardService>(), new FakeDashboardReader(model));

        Assert.Equal(3, ResultAssert.Success(await service.GetDashboard()).RequestSummary.MonthlyRequest);

        var failing = new DashboardService(Logger.For<DashboardService>(), new FakeDashboardReader(new InvalidOperationException()));
        ResultAssert.SystemError(await failing.GetDashboard());
    }

    [Fact]
    public async Task TotalActive_counts_seeded_records_and_empty_data()
    {
        using var db = new TestDb();
        await db.SaveAsync(
            Seed.User(1),
            Seed.User(2, email: "provider@gmail.com", role: "Service Provider"),
            Seed.User(3, email: "deleted@gmail.com", status: "Deleted"),
            Seed.Service(1, userId: 2, status: "Approved"),
            Seed.Service(2, userId: 2, status: "Approved"));
        db.Db.EmergencyServices.Find(2)!.Availability = "N";
        await db.Db.SaveChangesAsync();
        db.Db.ChangeTracker.Clear();

        var data = ResultAssert.Success(await new TotalActiveService(Logger.For<TotalActiveService>(), db.Db).GetTotalActive());

        Assert.Equal(1, data.Users);
        Assert.Equal(1, data.ServiceProviders);
        Assert.Equal(1, data.Services);
    }

    [Fact]
    public async Task TopTen_reports_return_seeded_and_empty_results()
    {
        using var db = new TestDb();
        await db.SaveAsync(
            Seed.Service(1),
            Seed.Service(2, serviceType: "Medical"),
            Seed.Request(1, userId: 1, serviceId: 1, townshipCode: "TSH", requestTime: DateTime.UtcNow.AddMinutes(-2)),
            Seed.Request(2, userId: 1, serviceId: 1, townshipCode: "TSH", requestTime: DateTime.UtcNow),
            Seed.Request(3, userId: 2, serviceId: 2, townshipCode: "OTHER"));

        var userRows = ResultAssert.Success(await new UserTopTenRequestService(Logger.For<UserTopTenRequestService>(), db.Db)
            .GetTopTenRequestPerUser(1));
        Assert.Equal(2, userRows.Count);
        Assert.Empty(ResultAssert.Success(await new UserTopTenRequestService(Logger.For<UserTopTenRequestService>(), db.Db)
            .GetTopTenRequestPerUser(999)));

        var townshipRows = ResultAssert.Success(await new TownshipTopTenRequestService(Logger.For<TownshipTopTenRequestService>(), db.Db)
            .GetTopTenRequestPerUser("NONE"));
        Assert.Empty(townshipRows);

        var serviceRows = ResultAssert.Success(await new TownshipTopTenServiceService(Logger.For<TownshipTopTenServiceService>(), db.Db)
            .GetTownshipTopTenService("TSH"));
        Assert.Equal(2, serviceRows.Single().RequestCount);
        Assert.Empty(ResultAssert.Success(await new TownshipTopTenServiceService(Logger.For<TownshipTopTenServiceService>(), db.Db)
            .GetTownshipTopTenService("NONE")));
    }

    private sealed class FakeDashboardReader : IDashboardReader
    {
        private readonly DashboardModel? _model;
        private readonly Exception? _exception;

        public FakeDashboardReader(DashboardModel model) => _model = model;
        public FakeDashboardReader(Exception exception) => _exception = exception;

        public Task<DashboardModel> ReadAsync()
        {
            if (_exception is not null)
                throw _exception;

            return Task.FromResult(_model!);
        }
    }
}
