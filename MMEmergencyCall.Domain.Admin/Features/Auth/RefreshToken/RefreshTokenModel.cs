using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.RefreshToken;

public class RefreshTokenModel
{
	public int UserId { get; set; }

	public Guid SessionId { get; set; }

	public string Name { get; set; } = null!;

	public string Email { get; set; } = null!;

	public DateTime? SessionExpiredTime { get; set; }

	public string Role { get; set; } = null!;

	public string Token { get; set; } = null!;
}
