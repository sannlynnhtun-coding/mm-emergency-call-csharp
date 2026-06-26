namespace MMEmergencyCall.Domain.Admin.Features.TownshipTopTenService;

[Route("api/Admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class TownshipTopTenServiceController : BaseController
{
	private readonly TownshipTopTenServiceService _townshipTopTenService;
	public TownshipTopTenServiceController(TownshipTopTenServiceService townshipTopTenServiceService)
	{
		_townshipTopTenService = townshipTopTenServiceService;
	}

	[HttpGet("{townshipCode}")]
	public async Task<IActionResult> GetTownshipTopTenService(string townshipCode)
	{
		var response = await _townshipTopTenService.GetTownshipTopTenService(townshipCode);

		return Execute(response);
	}
}
