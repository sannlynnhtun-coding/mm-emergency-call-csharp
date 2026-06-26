namespace MMEmergencyCall.Domain.Admin.Features.EmergencyRequestList;

[Route("api/admin/EmergencyRequestList")]
[AdminAuthorize]
[ApiController]
public class EmergencyRequestListController : BaseController
{
	private readonly EmergencyRequestListService _emergencyRequestService;

	public EmergencyRequestListController(EmergencyRequestListService emergencyRequestService)
	{
		_emergencyRequestService = emergencyRequestService;
	}

	[HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
	public async Task<IActionResult> GetEmergencyRequests(int? userId, string? serviceId, string? providerId,
		string? status, string? townshipCode, int pageNo = 1, int pageSize = 10)
	{
		if (pageNo <= 0 || pageSize <= 0)
		{
			return BadRequestResult("Page number and page size must be greater than zero.");
		}

		var model = await _emergencyRequestService.GetEmergencyRequests
			(pageNo, pageSize, userId, serviceId, providerId, status, townshipCode);
		return Execute(model);
	}
}
