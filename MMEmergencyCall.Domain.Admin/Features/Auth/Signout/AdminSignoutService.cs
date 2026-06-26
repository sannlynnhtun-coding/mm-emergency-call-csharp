using MMEmergencyCall.Domain.Admin.Features.RefreshToken;

namespace MMEmergencyCall.Domain.Admin.Features.Signout;

public class AdminSignoutService
{
	private readonly ILogger<AdminSignoutService> _logger;
	private readonly AppDbContext _db;
	public AdminSignoutService(ILogger<AdminSignoutService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<bool>> Signout(string token)
	{
		try
		{
			var requestTokenModel = token.ToDecrypt().ToObject<SignoutRefreshTokenModel>();

			var session = await _db.Sessions
				.FirstOrDefaultAsync(x => x.SessionId == requestTokenModel.SessionId);

			session.LogoutTime = DateTime.Now;
			_db.Entry(session).State = EntityState.Modified;
			await _db.SaveChangesAsync();

			return Result<bool>.Success(true);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<bool>.SystemError("Internal server error");
		}
	}
}
