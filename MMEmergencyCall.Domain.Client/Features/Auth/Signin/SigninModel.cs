namespace MMEmergencyCall.Domain.Client.Features.Signin;

public class SigninModel
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? SessionExpiredTime { get; set; }

    public string Token { get; set; } = null!;
}