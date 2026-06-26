namespace MMEmergencyCall.Domain.Client.Features.Register;

[Route("api/[controller]")]
[ApiController]
public class RegisterController : BaseController
{
    private readonly RegisterService _registerService;

    public RegisterController(RegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestModel requestModel)
    {
        return Execute(await _registerService.RegisterUserAsync(requestModel)); 
    }

    [HttpPost("/api/Client/Verify")]
    public async Task<IActionResult> Verify(VerifyRequestModel requestModel)
    {
        return Execute(await _registerService.VerifyUserAsync(requestModel));
    }
}