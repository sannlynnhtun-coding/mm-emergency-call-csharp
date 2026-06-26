using MMEmergencyCall.Domain.Client.Features.EmergencyRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequestHistory;

public class EmergencyRequestHistoryPaginationResponseModel
{
	public int PageNo { get; set; }
	public int PageSize { get; set; }
	public int PageCount { get; set; }
	public bool IsEndOfPage => PageNo == PageCount;
	public List<EmergencyRequestHistoryResponseModel> Data { get; set; }
}

public class EmergencyRequestHistoryResponseModel
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