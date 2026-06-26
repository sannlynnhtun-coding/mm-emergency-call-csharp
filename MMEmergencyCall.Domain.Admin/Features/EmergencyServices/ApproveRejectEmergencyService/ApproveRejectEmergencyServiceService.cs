using EnumServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus;

namespace MMEmergencyCall.Domain.Admin.Features.ApproveRejectEmergencyService;

public class ApproveRejectEmergencyServiceService
{
	private readonly ILogger<ApproveRejectEmergencyServiceService> _logger;

	private readonly AppDbContext _db;

	public ApproveRejectEmergencyServiceService(
		ILogger<ApproveRejectEmergencyServiceService> logger,
		AppDbContext db
	)
	{
		_logger = logger;
		_db = db;

	}
	public async Task<
	Result<bool>
> UpdateEmergencyServiceStatusAsync(int id, string serviceStatus)
	{
		if (!Enum.IsDefined(typeof(EnumServiceStatus), serviceStatus))
		{
			return Result<bool>.ValidationError(
				"Invalid Emergency Service Status. Status should be Pending, Approved, Rejected or Deleted"
			);
		}

		var item = await _db.EmergencyServices.FirstOrDefaultAsync(x => x.ServiceId == id);
		if (item is null)
		{
			return Result<bool>.NotFoundError(
				"This is no Emergency Service with Id: " + id
			);
		}

		item.ServiceStatus = serviceStatus;
		_db.Entry(item).State = EntityState.Modified;
		await _db.SaveChangesAsync();

		//var model = new EmergencyServiceResponseModel()
		//{
		//	ServiceId = id,
		//	ServiceStatus = serviceStatus
		//};

		return Result<bool>.Success(true);
	}
}
