namespace MMEmergencyCall.Domain.Admin.Features.SignIn;
public class AdminSignInModel
{
    public int UserId { get; set; }

    public Guid SessionId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? SessionExpiredTime { get; set; }

     public string Role { get; set; } = null!;

    public string Token { get; set; } = null!;
}