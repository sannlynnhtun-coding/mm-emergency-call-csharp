namespace MMEmergencyCall.Web.Models;

public sealed class AdminSigninRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class AdminSignInModel
{
    public int UserId { get; set; }
    public Guid SessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? SessionExpiredTime { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public sealed class RefreshTokenResponse
{
    public string Token { get; set; } = string.Empty;
}
