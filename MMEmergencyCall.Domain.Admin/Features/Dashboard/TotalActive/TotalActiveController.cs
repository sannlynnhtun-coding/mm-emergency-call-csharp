using MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.TotalActive;

[Route("api/admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class TotalActiveController : BaseController
{
	private readonly TotalActiveService _totalActiveService;

	public TotalActiveController(TotalActiveService totalActiveService)
	{
		_totalActiveService = totalActiveService;
	}

	[HttpGet]
	public async Task<IActionResult> GetTopTenRequestPerUser()
	{
		var result = await _totalActiveService.GetTotalActive();

		return Execute(result);
	}
}
