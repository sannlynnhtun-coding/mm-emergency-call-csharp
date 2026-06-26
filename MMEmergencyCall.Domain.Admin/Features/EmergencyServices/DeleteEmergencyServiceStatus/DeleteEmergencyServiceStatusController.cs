namespace MMEmergencyCall.Domain.Admin.Features.DeleteEmergencyServiceStatus;

[Route("api/admin/DeleteEmergencyServiceStatus")]
[AdminAuthorize]
[ApiController]
public class DeleteEmergencyServiceStatusController : BaseController
{
	private readonly DeleteEmergencyServiceStatusService _deleteEmergencyServicesService;

	public DeleteEmergencyServiceStatusController(DeleteEmergencyServiceStatusService deleteEmergencyServiceService)
	{
		_deleteEmergencyServicesService = deleteEmergencyServiceService;
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteEmergencyServiceAsync(int id)
	{
		var currentAdminId = HttpContext.GetCurrentAdminId();

		if (currentAdminId is null)
		{
			return UnauthorizedResult();
		}

		var model = await _deleteEmergencyServicesService.DeleteEmergencyServiceStatusAsync(id);

		return Execute(model);
	}
}
