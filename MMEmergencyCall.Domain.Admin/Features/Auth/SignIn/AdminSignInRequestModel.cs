namespace MMEmergencyCall.Domain.Admin.Features.SignIn;

public class AdminSigninRequestModel
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}