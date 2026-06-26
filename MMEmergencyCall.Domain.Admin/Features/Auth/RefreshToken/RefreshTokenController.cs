using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.RefreshToken;

[Route("api/Admin/[controller]")]
[AdminAuthorize]
[ApiController]
public class RefreshTokenController : BaseController
{
	private readonly RefreshTokenService _refreshTokenService;

	public RefreshTokenController(RefreshTokenService refreshTokenService)
	{
		_refreshTokenService = refreshTokenService;
	}

	[HttpPost]
	public async Task<IActionResult> RefreshToken()
	{
		var token = HttpContext.Request.Headers["Token"].ToString();

		var response = await _refreshTokenService.RefreshToken(token);

		return Execute(response);
	}
}
