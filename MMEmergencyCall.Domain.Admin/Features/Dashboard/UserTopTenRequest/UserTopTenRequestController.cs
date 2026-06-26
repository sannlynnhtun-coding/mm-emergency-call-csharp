using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;

[Route("api/admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class UserTopTenRequestController : BaseController
{
	private readonly UserTopTenRequestService _userTopTenRequestService;

	public UserTopTenRequestController(UserTopTenRequestService userTopTenRequestService)
	{
		_userTopTenRequestService = userTopTenRequestService;
	}

	[HttpGet("{userId}")]
	public async Task<IActionResult> GetTopTenRequestPerUser(int userId)
	{
		var result = await _userTopTenRequestService.GetTopTenRequestPerUser(userId);

		return Execute(result);
	}
}
