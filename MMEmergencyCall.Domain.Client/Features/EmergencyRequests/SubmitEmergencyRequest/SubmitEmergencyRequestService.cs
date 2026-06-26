using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.SubmitEmergencyRequest;

public class SubmitEmergencyRequestService
{
	private readonly AppDbContext _db;
	private readonly ILogger<SubmitEmergencyRequestService> _logger;
	public SubmitEmergencyRequestService(ILogger<SubmitEmergencyRequestService> logger, AppDbContext db)
	{
		_db = db;
		_logger = logger;
	}

	public async Task<Result<SubmitEmergencyRequestResponseModel>> AddEmergencyRequest(SubmitEmergencyRequestRequestModel request, int currentUserId)
	{
		try
		{
			if (request is null)
			{
				return Result<SubmitEmergencyRequestResponseModel>
					.ValidationError("Request model cannot be null.");
			}

			var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == currentUserId);
			if (user is null)
			{
				return Result<SubmitEmergencyRequestResponseModel>
					.ValidationError("Invalid User");
			}

			var townshipCode = string.IsNullOrEmpty(request.TownshipCode)
				? user.TownshipCode
				: request.TownshipCode;

			var validateRequestModelResponse = await ValidateEmergencyRequestRequestModel(request, townshipCode);

			if (validateRequestModelResponse is not null)
			{
				return validateRequestModelResponse;
			}

			var emergencyRequest = new EmergencyRequest()
			{
				UserId = currentUserId,
				ServiceId = request.ServiceId,
				ProviderId = request.ProviderId,
				RequestTime = DateTime.Now,
				Status = nameof(EnumEmergencyRequestStatus.Open),
				ResponseTime = null,
				Notes = request.Notes,
				TownshipCode = townshipCode
			};

			_db.EmergencyRequests.Add(emergencyRequest);
			await _db.SaveChangesAsync();

			var model = new SubmitEmergencyRequestResponseModel()
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

			return Result<SubmitEmergencyRequestResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			string message = "An error occurred while adding the emergency request : " + ex.Message;
			_logger.LogError(message);
			return Result<SubmitEmergencyRequestResponseModel>.SystemError("Internal server error");
		}
	}
	private async Task<Result<SubmitEmergencyRequestResponseModel>> ValidateEmergencyRequestRequestModel
		(SubmitEmergencyRequestRequestModel? request, string? townshipCode)
	{
		if (request is null)
		{
			return Result<SubmitEmergencyRequestResponseModel>
				.ValidationError("Request model cannot be null.");
		}

		if (request.ProviderId.HasValue && request.ProviderId < 1)
		{
			return Result<SubmitEmergencyRequestResponseModel>
				.ValidationError("Invalid Provider Id.");
		}

		if (!await IsApprovedServiceIdExist(request.ServiceId))
		{
			return Result<SubmitEmergencyRequestResponseModel>
				.ValidationError("Invalid or unapproved Service Id.");
		}

		if (!string.IsNullOrEmpty(townshipCode) && !await IsTownshipCodeExist(townshipCode))
		{
			return Result<SubmitEmergencyRequestResponseModel>
				.ValidationError("Invalid Township Code.");
		}

		return null;
	}

	private async Task<bool> IsUserIdExist(int userId)
	{
		var isUserIdExist = await _db.Users.AnyAsync(x => x.UserId == userId);
		return isUserIdExist;
	}

	private async Task<bool> IsApprovedServiceIdExist(int serviceId)
	{
		var isServiceIdExist = await _db.EmergencyServices.AnyAsync(x =>
			x.ServiceId == serviceId &&
			x.ServiceStatus == MMEmergencyCall.Shared.EnumServiceStatus.Approved.ToString());
		return isServiceIdExist;
	}

	private async Task<bool> IsTownshipCodeExist(string townshipCode)
	{
		var isTownshipCodeExist = await _db.Townships.AnyAsync(x => x.TownshipCode == townshipCode);
		return isTownshipCodeExist;
	}
}
