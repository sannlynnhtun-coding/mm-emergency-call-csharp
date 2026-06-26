namespace MMEmergencyCall.Domain.Client.Features.Register;

public class VerifyRequestModel
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}