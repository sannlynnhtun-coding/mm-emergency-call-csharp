namespace MMEmergencyCall.Domain.Admin.Features.RefreshToken;

public class RefreshTokenService
{
	private readonly ILogger<RefreshTokenService> _logger;
	private readonly AppDbContext _db;
	public RefreshTokenService(ILogger<RefreshTokenService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<RefreshTokenResponseModel>> RefreshToken(string token)
	{
		
		try
		{
			DateTime expireTime = DateTime.Now.AddMinutes(5);

			var requestTokenModel = token.ToDecrypt().ToObject<RefreshTokenModel>();

			var session = await _db.Sessions
				.FirstOrDefaultAsync(x => x.SessionId == requestTokenModel.SessionId);

			session!.ExpireTime = expireTime;

			_db.Entry(session).State = EntityState.Modified;
			await _db.SaveChangesAsync();

			RefreshTokenModel ResponseTokenModel = new()
			{
				UserId = requestTokenModel.UserId,
				SessionId = requestTokenModel.SessionId,
				Name = requestTokenModel.Name,
				Email = requestTokenModel.Email,
				SessionExpiredTime = expireTime,
				Role = requestTokenModel.Role,
			};

			var newToken = requestTokenModel.ToJson().ToEncrypt();
			requestTokenModel.Token = newToken;

			RefreshTokenResponseModel responseModel = new()
			{
				Token = newToken,
			};

			return Result<RefreshTokenResponseModel>.Success(responseModel);

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<RefreshTokenResponseModel>.SystemError("Internal server error");
		}
	}
}
