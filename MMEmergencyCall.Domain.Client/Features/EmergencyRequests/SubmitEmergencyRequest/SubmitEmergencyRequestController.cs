using MMEmergencyCall.Domain.Client.Features.EmergencyRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Client.Features.SubmitEmergencyRequest;

[Route("api/[controller]")]
[UserAuthorize]
[ApiController]
public class SubmitEmergencyRequestController : BaseController
{
	private readonly SubmitEmergencyRequestService _submitEmergencyRequestService;

	public SubmitEmergencyRequestController(SubmitEmergencyRequestService submitEmergencyRequestService)
	{
		_submitEmergencyRequestService = submitEmergencyRequestService;
	}
	[HttpPost]
	public async Task<IActionResult> AddEmergencyRequest(SubmitEmergencyRequestRequestModel request)
	{
		var currentUserId = HttpContext.GetCurrentUserId();

		if (!currentUserId.HasValue)
		{
			return UnauthorizedResult();
		}

		var model = await _submitEmergencyRequestService.AddEmergencyRequest(request, currentUserId.Value);
		return Execute(model);
	}
}
