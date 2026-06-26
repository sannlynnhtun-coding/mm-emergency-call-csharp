namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequestHistory;

[Route("api/[controller]")]
[UserAuthorize]
[ApiController]
public class EmergencyRequestHistoryController : BaseController
{
	private readonly EmergencyRequestHistoryService _emergencyRequestHistoryService;

	public EmergencyRequestHistoryController(EmergencyRequestHistoryService emergencyRequestHistoryService)
	{
		_emergencyRequestHistoryService = emergencyRequestHistoryService;
	}
	[HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
	public async Task<IActionResult> GetEmergencyRequests(string? serviceId, string? providerId,
		string? status, string? townshipCode, int pageNo = 1, int pageSize = 10)
	{
		var currentUserId = HttpContext.GetCurrentUserId();

		if (!currentUserId.HasValue)
		{
			return UnauthorizedResult();
		}

		if (pageNo <= 0 || pageSize <= 0)
		{
			return BadRequestResult("Page number and page size must be greater than zero.");
		}

		var model = await _emergencyRequestHistoryService.GetEmergencyRequests(pageNo, pageSize, currentUserId, serviceId, providerId, status, townshipCode);
		return Execute(model);
	}
}
