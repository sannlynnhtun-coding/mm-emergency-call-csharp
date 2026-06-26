using TBLEmergencyService = MMEmergencyCall.Databases.AppDbContextModels.EmergencyService;
using EnumServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus;

namespace MMEmergencyCall.Domain.Admin.Features.CreateEmergencyService;

public class CreateEmergencyServiceService
{
	private readonly ILogger<CreateEmergencyServiceService> _logger;

	private readonly AppDbContext _db;

	public CreateEmergencyServiceService(
		ILogger<CreateEmergencyServiceService> logger,
		AppDbContext db
	)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<CreateEmergencyServiceResponseModel>> CreateEmergencyServiceAsync(
	int currentUserId,
	CreateEmergencyServiceRequestModel request
)
	{
		try
		{
			//TODO Check validation

			var item = new TBLEmergencyService()
			{
				UserId = currentUserId,
				ServiceGroup = request.ServiceGroup,
				ServiceType = request.ServiceType,
				ServiceName = request.ServiceName,
				PhoneNumber = request.PhoneNumber,
				Location = request.Location,
				Availability = request.Availability,
				TownshipCode = request.TownshipCode,
				ServiceStatus = EnumServiceStatus.Pending.ToString()
			};

			_db.EmergencyServices.Add(item);
			await _db.SaveChangesAsync();

			var response = new CreateEmergencyServiceResponseModel()
			{
				ServiceId = item.ServiceId,
				UserId = item.UserId,
				ServiceGroup = item.ServiceGroup,
				ServiceType = item.ServiceType,
				ServiceName = item.ServiceName,
				PhoneNumber = item.PhoneNumber,
				Location = item.Location,
				Availability = item.Availability,
				TownshipCode = item.TownshipCode,
				ServiceStatus = item.ServiceStatus
			};

			return Result<CreateEmergencyServiceResponseModel>.Success(response);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<CreateEmergencyServiceResponseModel>.SystemError("Internal server error");
		}
	}
}
