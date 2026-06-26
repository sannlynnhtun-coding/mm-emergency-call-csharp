namespace MMEmergencyCall.Domain.Admin.Features.CompleteCloseCancelEmergencyRequest;

[Route("api/admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class CompleteCloseCancelEmergencyRequestController : BaseController
{
	private readonly CompleteCloseCancelEmergencyRequestService _updateEmergencyRequestStatusService;

	public CompleteCloseCancelEmergencyRequestController(CompleteCloseCancelEmergencyRequestService updateEmergencyRequestStatusService)
	{
		_updateEmergencyRequestStatusService = updateEmergencyRequestStatusService;
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateEmergencyRequestStatus(int id, CompleteCloseCancelEmergencyRequestRequestModel statusRequest)
	{
		var model = await _updateEmergencyRequestStatusService.UpdateEmergencyRequestStatus(id, statusRequest);
		return Execute(model);
	}
}
