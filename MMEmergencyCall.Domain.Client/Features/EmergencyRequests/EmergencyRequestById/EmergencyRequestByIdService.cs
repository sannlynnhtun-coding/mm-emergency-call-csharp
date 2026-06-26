using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequestById;

public class EmergencyRequestByIdService
{

	private readonly ILogger<EmergencyRequestByIdService> _logger;
	private readonly AppDbContext _db;

	public EmergencyRequestByIdService(
		ILogger<EmergencyRequestByIdService> logger, 
		AppDbContext context
		)
	{
		_logger = logger;
		_db = context;
	}

	public async Task<Result<EmergencyRequestByIdResponseModel>> GetEmergencyRequestById(int id, int? userId)
	{
		try
		{
			var emergencyRequest = await _db.EmergencyRequests.Where(x => x.RequestId == id && x.UserId == userId)
				.FirstOrDefaultAsync();

			if (emergencyRequest is null)
			{
				return Result<EmergencyRequestByIdResponseModel>
						.NotFoundError("Emergency Request with Id " + id + " not found.");
			}

			var model = new EmergencyRequestByIdResponseModel()
			{
				RequestId = emergencyRequest.RequestId,
				UserId = emergencyRequest.UserId,
				ServiceId = emergencyRequest.ServiceId,
				ProviderId = emergencyRequest.ProviderId,
				RequestTime = emergencyRequest.RequestTime,
				Status = emergencyRequest.Status,
				ResponseTime = emergencyRequest.ResponseTime,
				Notes = emergencyRequest.Notes,
				TownshipCode = emergencyRequest.TownshipCode
			};

			return Result<EmergencyRequestByIdResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			string message = "An error occurred while getting the emergency request with id " + id + " : " + ex.Message;
			_logger.LogError(message);
			return Result<EmergencyRequestByIdResponseModel>.SystemError("Internal server error");
		}
	}
}
