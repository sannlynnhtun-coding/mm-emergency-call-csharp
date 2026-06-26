namespace MMEmergencyCall.Domain.Client.Features.Signin;

[Route("api/[controller]")]
[ApiController
    ]
public class SigninController : BaseController
{
    private readonly SigninService _signinService;

    public SigninController(SigninService signinService)
    {
        _signinService = signinService;
    }

    [HttpPost]
    public async Task<IActionResult> SigninAsync(SigninRequestModel requestModel)
    {
        var response = await _signinService.SigninAsync(requestModel);
        return Execute(response);
    }
}
