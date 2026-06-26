namespace MMEmergencyCall.Domain.Admin.Features.UpdateUser;

[Route("api/Admin/UpdateUser")]
[AdminAuthorize]
[ApiController]
public class UpdateUserController : BaseController
{
	private readonly UpdateUserService _updateUserService;

	public UpdateUserController(UpdateUserService updateUserService)
	{
		_updateUserService = updateUserService;
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateUserAsync(int id, UpdateUserRequestModel requestModel)
	{
		var model = await _updateUserService.UpdateUserAsync(id, requestModel);
		return Execute(model);
	}
}
