namespace MMEmergencyCall.Domain.Admin.Features.Dashboard;

public class DashboardService
{
	private readonly ILogger<DashboardService> _logger;
	private readonly IDashboardReader _dashboardReader;
	public DashboardService(ILogger<DashboardService> logger, IDashboardReader dashboardReader)
	{
		_logger = logger;
		_dashboardReader = dashboardReader;
	}

	public async Task<Result<DashboardModel>> GetDashboard()
	{
		try
		{
			var dashboardData = await _dashboardReader.ReadAsync();

			return Result<DashboardModel>.Success(dashboardData);

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<DashboardModel>.SystemError("Internal server error");
		}
	}
}

