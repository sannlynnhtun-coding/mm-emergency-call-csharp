namespace MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;

public class UserTopTenRequestResponseModel
{
	public int RequestId { get; set; }

	public int UserId { get; set; }

	public int ServiceId { get; set; }

	public int? ProviderId { get; set; }

	public DateTime RequestTime { get; set; }

	public string Status { get; set; } = null!;

	public DateTime? ResponseTime { get; set; }

	public string? Notes { get; set; }

	public string? TownshipCode { get; set; }
}
