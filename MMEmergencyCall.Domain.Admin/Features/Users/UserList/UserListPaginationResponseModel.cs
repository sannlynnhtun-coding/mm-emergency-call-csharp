namespace MMEmergencyCall.Domain.Admin.Features.UserList;

public class UserListPaginationResponseModel
{
	public int PageNo { get; set; }

	public int PageSize { get; set; }

	public int PageCount { get; set; }

	public bool IsEndOfPage => PageNo == PageCount;

	public List<UserListResponseModel> Data { get; set; }
}
