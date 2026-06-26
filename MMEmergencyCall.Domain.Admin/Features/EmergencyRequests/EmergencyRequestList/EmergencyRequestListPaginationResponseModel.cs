namespace MMEmergencyCall.Domain.Admin.Features.EmergencyRequestList;

public class EmergencyRequestListPaginationResponseModel
{
	public int PageNo { get; set; }
	public int PageSize { get; set; }
	public int PageCount { get; set; }
	public bool IsEndOfPage => PageNo >= PageCount;
	public List<EmergencyRequestListModel> Data { get; set; }
}
