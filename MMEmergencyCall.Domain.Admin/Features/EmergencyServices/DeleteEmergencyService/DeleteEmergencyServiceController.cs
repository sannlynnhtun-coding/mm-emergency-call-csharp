
namespace MMEmergencyCall.Domain.Admin.Features.DeleteEmergencyService;

[Route("api/admin/DeleteEmergencyService")]
[AdminAuthorize]
[ApiController]
public class DeleteEmergencyServiceController : BaseController
{
	private readonly DeleteEmergencyServiceService _deleteEmergencyServicesService;

	public DeleteEmergencyServiceController(DeleteEmergencyServiceService deleteEmergencyServiceService)
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

		var responseModel = await _deleteEmergencyServicesService
			.DeleteEmergencyServiceAsync(id);

		return Execute(responseModel);
	}
}
