namespace MMEmergencyCall.Domain.Client.Features.Logout;

public class LogoutRequestModel
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
}
