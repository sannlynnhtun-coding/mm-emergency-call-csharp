using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Admin.Features.CreateEmergencyService;

public class CreateEmergencyServiceRequestModel
{
	public string ServiceGroup { get; set; } = null!;

	public string ServiceType { get; set; } = null!;

	public string ServiceName { get; set; } = null!;

	public string PhoneNumber { get; set; } = null!;

	public string? Location { get; set; }

	[MaxLength(1)]
	public string? Availability { get; set; }

	public string? TownshipCode { get; set; }
}
