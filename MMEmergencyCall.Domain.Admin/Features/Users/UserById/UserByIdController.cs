namespace MMEmergencyCall.Domain.Admin.Features.UserById;

[Route("api/admin/UserById")]
[AdminAuthorize]
[ApiController]
public class UserByIdController : BaseController
{
	private readonly UserByIdService _userByIdService;

	public UserByIdController(UserByIdService userByIdService)
	{
		_userByIdService = userByIdService;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetByIdAsync(int id)
	{
		var model = await _userByIdService.GetByIdAsync(id);
		return Execute(model);
	}
}
