namespace MMEmergencyCall.Domain.Admin.Features.CompleteCloseCancelEmergencyRequest;

public class CompleteCloseCancelEmergencyRequestService
{
	private readonly ILogger<CompleteCloseCancelEmergencyRequestService> _logger;
	private readonly AppDbContext _db;

	public CompleteCloseCancelEmergencyRequestService(ILogger<CompleteCloseCancelEmergencyRequestService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<bool>> UpdateEmergencyRequestStatus(int id, CompleteCloseCancelEmergencyRequestRequestModel statusRequest)
	{
		try
		{
			if (!Enum.IsDefined(typeof(EnumEmergencyRequestStatus), statusRequest.Status))
			{
				return Result<bool>.ValidationError(
					"Invalid Emergency Request Status. Status should be Open, Cancel, Closed or Completed"
				);
			}

			var existingEmergencyRequest = await _db.EmergencyRequests
				.FirstOrDefaultAsync(x => x.RequestId == id);

			if (existingEmergencyRequest is null)
			{
				return Result<bool>
					  .NotFoundError("Emergency Request with Id " + id + " not found.");
			}

			existingEmergencyRequest.Status = statusRequest.Status;
			_db.Entry(existingEmergencyRequest).State = EntityState.Modified;
			await _db.SaveChangesAsync();

			return Result<bool>.Success(true);
		}
		catch (Exception ex)
		{
			string message = "An error occurred while updating the status of emergency request with id " + id + " : " +
							 ex.Message;
			_logger.LogError(message);
			return Result<bool>.SystemError("Internal server error");
		}
	}
}
