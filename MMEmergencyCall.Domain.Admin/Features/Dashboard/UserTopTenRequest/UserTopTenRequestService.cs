namespace MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;

public class UserTopTenRequestService
{
	private readonly ILogger<UserTopTenRequestService> _logger;

	private readonly AppDbContext _db;

	public UserTopTenRequestService(ILogger<UserTopTenRequestService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<List<UserTopTenRequestResponseModel>>> GetTopTenRequestPerUser(int userId)
	{
		try
		{
			var emergencyRequests = await _db.EmergencyRequests
				.Where(x => x.UserId == userId)
				.OrderByDescending(x => x.RequestTime)
				.Take(10)
			.ToListAsync();

			var responseData = emergencyRequests.Select(x => new UserTopTenRequestResponseModel
			{
				RequestId = x.RequestId,
				UserId = x.UserId,
				ServiceId = x.ServiceId,
				ProviderId = x.ProviderId,
				RequestTime = x.RequestTime,
				Status = x.Status,
				ResponseTime = x.ResponseTime,
				Notes = x.Notes,
				TownshipCode = x.TownshipCode
			}).ToList();

			return Result<List<UserTopTenRequestResponseModel>>.Success(responseData);

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<List<UserTopTenRequestResponseModel>>.SystemError("Internal server error");
		}
	}
}
