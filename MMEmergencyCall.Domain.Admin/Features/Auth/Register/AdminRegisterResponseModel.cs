namespace MMEmergencyCall.Domain.Admin.Features.Register;

public class AdminRegisterResponseModel
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? EmergencyType { get; set; }
    public string? EmergencyDetails { get; set; }
    public string TownshipCode { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string UserStatus { get; set; } = null!;
    public string IsVerified { get; set; } = null!;
}
