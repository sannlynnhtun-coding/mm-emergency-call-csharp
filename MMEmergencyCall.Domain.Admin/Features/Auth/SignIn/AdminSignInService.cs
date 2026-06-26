namespace MMEmergencyCall.Domain.Admin.Features.SignIn;

public class AdminSigninService
{
	private readonly ILogger<User> _logger;

	private readonly AppDbContext _db;

	public AdminSigninService(ILogger<User> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<AdminSignInModel>> SigninAsync(AdminSigninRequestModel requestModel)
	{
		try
		{
			var email = requestModel.Email;
			var password = requestModel.Password;


			var user = await _db.Users
					   .Where(u => u.Email == requestModel.Email
					   && u.Password == requestModel.Password && u.Role.ToLower() == "admin")
					   .FirstOrDefaultAsync();

			if (user is null)
			{
				return Result<AdminSignInModel>.ValidationError("Username or Password is incorrect.");
			}

			if (user.IsVerified == EnumVerify.N.ToString()
				|| user.UserStatus == EnumUserStatus.Deleted.ToString()
				)
			{
				return Result<AdminSignInModel>.ValidationError("Can't signin to account.");
			}

			Guid sessionId = Guid.NewGuid();
			DateTime expireTime = DateTime.Now.AddMinutes(5);

			Session session = new()
			{
				SessionId = sessionId,
				UserId = user.UserId,
				ExpireTime = expireTime,
			};
			await _db.Sessions.AddAsync(session);
			await _db.SaveChangesAsync();

			AdminSignInModel signinModel = new()
			{
				Email = user.Email,
				Name = user.Name,
				SessionExpiredTime = expireTime,
				Role = user.Role,
				UserId = user.UserId,
				SessionId = sessionId
			};

			var token = signinModel.ToJson().ToEncrypt();
			signinModel.Token = token;

			return Result<AdminSignInModel>.Success(signinModel);

		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<AdminSignInModel>.SystemError("Internal server error");
		}
	}
}
