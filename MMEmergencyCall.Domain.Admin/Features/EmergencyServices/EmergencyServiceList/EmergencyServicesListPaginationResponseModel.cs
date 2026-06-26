namespace MMEmergencyCall.Domain.Admin.Features.EmergencyServiceList;

public class EmergencyServicesListPaginationResponseModel
{
	public int PageNo { get; set; }

	public int PageSize { get; set; }

	public int PageCount { get; set; }

	public bool IsEndOfPage => PageNo == PageCount;

	public List<EmergencyServiceListResponseModel> Data { get; set; }
}
