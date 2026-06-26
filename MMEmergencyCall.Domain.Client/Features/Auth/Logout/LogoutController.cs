namespace MMEmergencyCall.Domain.Client.Features.Logout
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : BaseController
    {
        private readonly LogoutService _logoutService;

        public LogoutController(LogoutService logoutService)
        {
            _logoutService = logoutService;
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync(LogoutRequestModel requestModel)
        {
            var response = await _logoutService.LogoutAsync(requestModel);
            return Execute(response);
        }
    }
}
