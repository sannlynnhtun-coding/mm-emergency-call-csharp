namespace MMEmergencyCall.Domain.Admin.Features.DeleteUser;

[Route("api/[controller]")]
[AdminAuthorize]
[ApiController]
public class DeleteUserController : BaseController
{
	private readonly DeleteUserService _deleteUserService;

	public DeleteUserController(DeleteUserService deleteUserService)
	{
		_deleteUserService = deleteUserService;
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteUserAsync(int id)
	{
		var model = await _deleteUserService.DeleteUserAsync(id);
		return Execute(model);
	}
}
