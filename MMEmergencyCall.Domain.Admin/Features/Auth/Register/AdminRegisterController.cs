namespace MMEmergencyCall.Domain.Admin.Features.Register;

[Route("api/admin/[controller]")]
[ApiController]
public class AdminRegisterController : BaseController
{
    private readonly AdminRegisterService _registerService;

    public AdminRegisterController(AdminRegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(AdminRegisterRequestModel requestModel)
    {
        return Execute(await _registerService.RegisterAdminAsync(requestModel));
    }
}
