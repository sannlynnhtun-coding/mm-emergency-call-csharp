namespace MMEmergencyCall.Domain.Admin.Features.CreateEmergencyService;

[Route("api/admin/CreateEmergencyService")]
[AdminAuthorize]
[ApiController]
public class CreateEmergencyServiceController: BaseController
{
	private readonly CreateEmergencyServiceService _createEmergencyServicesService;

	public CreateEmergencyServiceController(CreateEmergencyServiceService createEmergencyServiceService)
	{
		_createEmergencyServicesService = createEmergencyServiceService;
	}

	[HttpPost]
	public async Task<IActionResult> CreateEmergencyServiceAsync(
		CreateEmergencyServiceRequestModel request
	)
	{
		var currentUserId = HttpContext.GetCurrentAdminId();

		if (!currentUserId.HasValue)
		{
			return UnauthorizedResult();
		}
		var userId = Convert.ToInt32(currentUserId);

		var response = await _createEmergencyServicesService.CreateEmergencyServiceAsync(
			userId,
			request
		);
		return Execute(response);
	}
}
