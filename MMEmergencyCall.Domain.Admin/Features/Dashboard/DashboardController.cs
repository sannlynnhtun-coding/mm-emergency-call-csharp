namespace MMEmergencyCall.Domain.Admin.Features.Dashboard;

[Route("api/Admin/Dashboard")]
[AdminAuthorize]
[ApiController]
public class DashboardController : BaseController
{
	private readonly DashboardService _dashboardService;
	public DashboardController(DashboardService dashboardService)
	{
		_dashboardService = dashboardService;
	}

	[HttpGet]
	public async Task<IActionResult> GetDashboard()
	{
		var result = await _dashboardService.GetDashboard();
		return Execute(result);
	}
}
