namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequestById;

[Route("api/[controller]")]
[UserAuthorize]
[ApiController]
public class EmergencyRequestByIdController : BaseController
{
	private readonly EmergencyRequestByIdService _emergencyRequestByIdService;

	public EmergencyRequestByIdController(EmergencyRequestByIdService emergencyRequestByIdService)
	{
		_emergencyRequestByIdService = emergencyRequestByIdService;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetEmergencyRequestById(int id)
	{
		var currentUserId = HttpContext.GetCurrentUserId();

		if (!currentUserId.HasValue)
		{
			return UnauthorizedResult();
		}

		var model = await _emergencyRequestByIdService.GetEmergencyRequestById(id, currentUserId.Value);
		return Execute(model);
	}
}
