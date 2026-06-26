namespace MMEmergencyCall.Domain.Admin.Features.ApproveRejectEmergencyService;

[Route("api/Admin/ApproveOrRejectEmergencyServiceStatus")]
[AdminAuthorize]
[ApiController]
public class ApproveRejectEmergencyServiceController : BaseController
{
	private readonly ApproveRejectEmergencyServiceService _updateEmergencyServiceStatusService;

	public ApproveRejectEmergencyServiceController(ApproveRejectEmergencyServiceService updateEmergencyServiceStatusService)
	{
		_updateEmergencyServiceStatusService = updateEmergencyServiceStatusService;
	}

	[HttpPatch("{id}")]
	public async Task<IActionResult> UpdateEmergencyServiceStatusAsync(int id, string serviceStatus)
	{
		var model = await _updateEmergencyServiceStatusService.UpdateEmergencyServiceStatusAsync(
			id,
			serviceStatus
		);

		return Execute(model);
	}
}
