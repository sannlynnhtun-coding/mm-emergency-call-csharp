namespace MMEmergencyCall.Domain.Admin.Features.TownshipTopTenRequest;

[Route("api/admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class TownshipTopTenRequestController : BaseController
{
	private readonly TownshipTopTenRequestService _townshipTopTenRequestService;

	public TownshipTopTenRequestController(TownshipTopTenRequestService townshipTopTenRequestService)
	{
		_townshipTopTenRequestService = townshipTopTenRequestService;
	}

	[HttpGet("townshipCode")]
	public async Task<IActionResult> GetTopTenRequestPerUser(string townshipCode)
	{
		var result = await _townshipTopTenRequestService.GetTopTenRequestPerUser(townshipCode);

		return Execute(result);
	}
}
