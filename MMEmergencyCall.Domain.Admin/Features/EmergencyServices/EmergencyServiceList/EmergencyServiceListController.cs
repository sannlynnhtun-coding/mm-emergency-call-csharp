namespace MMEmergencyCall.Domain.Admin.Features.EmergencyServiceList;

[Route("api/Admin/EmergencyService")]
[AdminAuthorize]
[ApiController]
public class EmergencyServiceListController : BaseController
{
	private readonly EmergencyServiceListService _emergencyServicesService;

	public EmergencyServiceListController(EmergencyServiceListService emergencyServiceService)
	{
		_emergencyServicesService = emergencyServiceService;
	}

	[HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
	public async Task<IActionResult> GetEmergencyServicesByStatusAsync(
		int pageNo,
		int pageSize,
		string? serviceStatus
	)
	{
		var response = await _emergencyServicesService.GetEmergencyServicesByStatusAsync(
			serviceStatus,
			pageNo,
			pageSize
		);

		return Execute(response);
	}
}
