namespace MMEmergencyCall.Domain.Admin.Features.Signout;

[Route("api/Admin/Signout")]
[AdminAuthorize]
[ApiController]
public class AdminSignoutController : BaseController
{
	private readonly AdminSignoutService _adminSignoutService;
	public AdminSignoutController(AdminSignoutService adminSignoutService)
	{
		_adminSignoutService = adminSignoutService;
	}

	[HttpPost]
	public async Task<IActionResult> Signout()
	{
		string token = HttpContext.Request.Headers["Token"].ToString();

		var response = await _adminSignoutService.Signout(token);

		return Execute(response);
	}
}
