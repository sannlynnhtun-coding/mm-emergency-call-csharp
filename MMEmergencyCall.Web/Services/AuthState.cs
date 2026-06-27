using MMEmergencyCall.Web.Models;

namespace MMEmergencyCall.Web.Services;

public sealed class AuthState
{
    public AdminSignInModel? User { get; private set; }
    public string? Token => User?.Token;
    public bool IsSignedIn => !string.IsNullOrWhiteSpace(Token);

    public void SignIn(AdminSignInModel user)
    {
        User = user;
    }

    public void SignOut()
    {
        User = null;
    }
}
