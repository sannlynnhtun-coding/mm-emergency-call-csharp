using MMEmergencyCall.Domain.Admin.Features.UserTopTenRequest;

namespace MMEmergencyCall.Domain.Admin.Features.TotalActive;

public class TotalActiveService
{
	private readonly ILogger<TotalActiveService> _logger;

	private readonly AppDbContext _db;

	public TotalActiveService(ILogger<TotalActiveService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<TotalActiveResponseModel>> GetTotalActive()
	{
		try
		{
			var userCount = await _db.Users
			.Where(u => u.Role == "Normal User" && u.UserStatus == EnumUserStatus.Activated.ToString())
			.CountAsync();

			var serviceProviderCount = await _db.Users
				.Where(u => u.Role == "Service Provider" && u.UserStatus == EnumUserStatus.Activated.ToString())
				.CountAsync();

			var serviceCount = await _db.EmergencyServices
				.Where(x => x.Availability == "Y")
				.CountAsync();

			TotalActiveResponseModel totalActiveResponseModel = new()
			{
				Users = userCount,
				ServiceProviders = serviceProviderCount,
				Services = serviceCount
			};

			return Result<TotalActiveResponseModel>.Success(totalActiveResponseModel);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<TotalActiveResponseModel>.SystemError("Internal server error");
		}
	}
}
