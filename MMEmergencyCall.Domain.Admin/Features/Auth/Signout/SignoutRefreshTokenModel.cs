namespace MMEmergencyCall.Domain.Admin.Features.Signout;

public class SignoutRefreshTokenModel
{
	public int UserId { get; set; }

	public Guid SessionId { get; set; }

	public string Name { get; set; } = null!;

	public string Email { get; set; } = null!;

	public DateTime? SessionExpiredTime { get; set; }

	public string Role { get; set; } = null!;

	public string Token { get; set; } = null!;
}
