namespace MMEmergencyCall.Domain.Admin.Features.CreateUser;

public class CreateUserService
{
	private readonly ILogger<CreateUserService> _logger;
	private readonly AppDbContext _context;

	public CreateUserService(ILogger<CreateUserService> logger, AppDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<Result<CreateUserResponseModel>> CreateUserAsync(CreateUserRequestModel request)
	{
		try
		{
			var user = new User
			{
				Name = request.Name,
				Email = request.Email,
				Password = request.Password,
				PhoneNumber = request.PhoneNumber,
				Address = request.Address,
				EmergencyType = request.EmergencyType,
				EmergencyDetails = request.EmergencyDetails,
				TownshipCode = request.TownshipCode,
				Role = request.Role
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			var model = new CreateUserResponseModel
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

			return Result<CreateUserResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			var message = "An error occurred while creating User: " + ex.ToString();
			_logger.LogError(message);
			return Result<CreateUserResponseModel>.SystemError("Internal server error");
		}
	}
}
