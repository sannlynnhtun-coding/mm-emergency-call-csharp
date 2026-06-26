using Dapper;
using MMEmergencyCall.Databases.Dapper;
using System.Data;

namespace MMEmergencyCall.Domain.Admin.Features.Dashboard;

public interface IDashboardReader
{
    Task<DashboardModel> ReadAsync();
}

public class DapperDashboardReader : IDashboardReader
{
    private readonly DapperContext _dapperContext;

    public DapperDashboardReader(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<DashboardModel> ReadAsync()
    {
        using IDbConnection connection = _dapperContext.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            "sp_Dashboard_Process",
            new { UserId = 0 },
            commandType: CommandType.StoredProcedure);

        return new DashboardModel
        {
            RequestSummary = await multi.ReadFirstOrDefaultAsync<RequestSummaryModel>(),
            TopTenServicePerTownship = (await multi.ReadAsync<TopTenServicePerTownshipModel>()).AsList(),
            ServiceProviderActivity = (await multi.ReadAsync<ServiceProviderActivityModel>()).AsList(),
            TopTenRequestPerUser = (await multi.ReadAsync<TopTenRequestPerUserModel>()).AsList()
        };
    }
}
