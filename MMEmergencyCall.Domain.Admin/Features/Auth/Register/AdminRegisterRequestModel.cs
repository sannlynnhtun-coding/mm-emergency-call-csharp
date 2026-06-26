namespace MMEmergencyCall.Domain.Admin.Features.Register;

public class AdminRegisterRequestModel
{
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string TownshipCode { get; set; } = null!;
    public string? EmergencyType { get; set; }
    public string? EmergencyDetails { get; set; }
}