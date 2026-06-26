namespace MMEmergencyCall.Domain.Admin.Features.UserById;

public class UserByIdService
{
	private readonly ILogger<UserByIdService> _logger;
	private readonly AppDbContext _context;

	public UserByIdService(ILogger<UserByIdService> logger, AppDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<Result<UserByIdResponseModel>> GetByIdAsync(int id)
	{
		try
		{
			var user = await _context.Users.FindAsync(id);
			if (user is null)
				return Result<UserByIdResponseModel>.NotFoundError("User not found.");

			var model = new UserByIdResponseModel()
			{

				UserId = user.UserId,
				Name = user.Name,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Address = user.Address,
				EmergencyType = user.EmergencyType,
				EmergencyDetails = user.EmergencyDetails,
				TownshipCode = user.TownshipCode,
				Role = user.Role,
				UserStatus = user.UserStatus
			};

			return Result<UserByIdResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			string message = "An error occurred while getting the user requests for id " + id + " : " + ex.Message;
			_logger.LogError(message);
			return Result<UserByIdResponseModel>.SystemError("Internal server error");
		}
	}
}
