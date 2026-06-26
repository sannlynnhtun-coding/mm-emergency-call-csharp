using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.CreateUser;

public class CreateUserRequestModel
{
	public int UserId { get; set; }

	public string Name { get; set; }

	public string Email { get; set; }

	public string Password { get; set; }

	public string PhoneNumber { get; set; }

	public string Address { get; set; }

	public string? EmergencyType { get; set; }

	public string? EmergencyDetails { get; set; }

	public string TownshipCode { get; set; }

	public string Role { get; set; }

	public string UserStatus { get; set; } = "Pending";
}
