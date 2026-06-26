namespace MMEmergencyCall.Domain.Admin.Features.UserList;

[Route("api/admin/UserList")]
[AdminAuthorize]
[ApiController]
public class UserListController : BaseController
{
	private readonly UserListService _userListService;

	public UserListController(UserListService userListService)
	{
		_userListService = userListService;
	}

	[HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
	public async Task<IActionResult> GetUsersByRoleAsync(string? role, string? userStatus, int pageNo = 1, int pageSize = 10)
	{
		var model = await _userListService.GetUsersAsync(pageNo, pageSize, role, userStatus);
		return Execute(model);
	}
}
