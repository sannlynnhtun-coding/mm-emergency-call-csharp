namespace MMEmergencyCall.Domain.Admin.Features.CreateUser;

[Route("api/admin/CreateUser")]
[AdminAuthorize]
[ApiController]
public class CreateUserController : BaseController
{
	private readonly CreateUserService _createUserService;

	public CreateUserController(CreateUserService createUserService)
	{
		_createUserService = createUserService;
	}

	[HttpPost]
	public async Task<IActionResult> CreateUserAsync(CreateUserRequestModel requestModel)
	{
		var model = await _createUserService.CreateUserAsync(requestModel);
		return Execute(model);
	}
}
