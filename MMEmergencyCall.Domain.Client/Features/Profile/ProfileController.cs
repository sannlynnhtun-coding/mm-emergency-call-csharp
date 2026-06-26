namespace MMEmergencyCall.Domain.Client.Features.Profile;

[Route("api/[controller]")]
[UserAuthorize]
[ApiController]
public class ProfileController : BaseController
{
    private readonly ProfileService _profileService;

    public ProfileController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _profileService.GetProfileById(currentUserId.Value);
        return Execute(model);
    }

    [HttpDelete]
    public async Task<IActionResult> DeactivateProfile()
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _profileService.DeactivateProfile(currentUserId.Value);
        return Execute(model);
    }
}
