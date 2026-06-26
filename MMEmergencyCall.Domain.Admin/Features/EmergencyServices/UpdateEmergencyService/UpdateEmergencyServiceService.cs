namespace MMEmergencyCall.Domain.Admin.Features.UpdateEmergencyService;

public class UpdateEmergencyServiceService
{
	private readonly ILogger<UpdateEmergencyServiceService> _logger;

	private readonly AppDbContext _db;

	public UpdateEmergencyServiceService(
		ILogger<UpdateEmergencyServiceService> logger,
		AppDbContext db
	)
	{
		_logger = logger;
		_db = db;
	}


	public async Task<Result<UpdateEmergencyServiceResponseModel>> UpdateEmergencyServiceAsync(
	int id,
	UpdateEmergencyServiceRequestModel requestModel
)
	{
		try
		{
			var item = await _db.EmergencyServices.FirstOrDefaultAsync(x => x.ServiceId == id);
			if (item is null)
			{
				return Result<UpdateEmergencyServiceResponseModel>.NotFoundError(
					"This is no Emergency Service with Id: " + id
				);
			}

			item.ServiceGroup = requestModel.ServiceGroup;
			item.ServiceType = requestModel.ServiceType;
			item.ServiceName = requestModel.ServiceName;
			item.PhoneNumber = requestModel.PhoneNumber;
			item.Location = requestModel.Location;
			item.Availability = requestModel.Availability;
			item.TownshipCode = requestModel.TownshipCode;
			_db.Entry(item).State = EntityState.Modified;
			await _db.SaveChangesAsync();

			UpdateEmergencyServiceResponseModel responseModel = new()
			{
				ServiceId = item.ServiceId,
				UserId = item.UserId,
				ServiceGroup = requestModel.ServiceGroup,
				ServiceType = requestModel.ServiceType,
				ServiceName = requestModel.ServiceName,
				PhoneNumber = requestModel.PhoneNumber,
				Location = requestModel.Location,
				Availability = requestModel.Availability,
				TownshipCode = requestModel.TownshipCode,
				ServiceStatus = item.ServiceStatus
			};

			return Result<UpdateEmergencyServiceResponseModel>.Success(responseModel);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<UpdateEmergencyServiceResponseModel>.SystemError("Internal server error");
		}
	}
}
