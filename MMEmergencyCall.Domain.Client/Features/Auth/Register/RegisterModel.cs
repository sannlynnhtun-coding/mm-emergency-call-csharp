namespace MMEmergencyCall.Domain.Client.Features.Register;

public class RegisterModel
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public string? EmergencyType { get; set; }

    public string? EmergencyDetails { get; set; }

    public string TownshipCode { get; set; }

    public string Role { get; set; }

    public string UserStatus { get; set; }

    public string IsVerified { get; set; }
    public string Otp { get; set; }
}
