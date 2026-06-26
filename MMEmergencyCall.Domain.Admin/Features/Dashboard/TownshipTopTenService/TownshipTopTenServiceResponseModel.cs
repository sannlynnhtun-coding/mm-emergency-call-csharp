namespace MMEmergencyCall.Domain.Admin.Features.TownshipTopTenService;

public class TownshipTopTenServiceResponseModel
{
	public int ServiceId { get; set; }

	public int UserId { get; set; }

	public string ServiceGroup { get; set; } = null!;

	public string ServiceType { get; set; } = null!;

	public string ServiceName { get; set; } = null!;

	public string PhoneNumber { get; set; } = null!;

	public string? Location { get; set; }

	public string? Availability { get; set; }

	public string? TownshipCode { get; set; }

	public string? ServiceStatus { get; set; }
	public int RequestCount { get; set; }
}
