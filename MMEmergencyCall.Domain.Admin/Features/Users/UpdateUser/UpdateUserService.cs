namespace MMEmergencyCall.Domain.Admin.Features.UpdateUser;

public class UpdateUserService
{
	private readonly ILogger<UpdateUserService> _logger;
	private readonly AppDbContext _context;

	public UpdateUserService(ILogger<UpdateUserService> logger, AppDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<Result<UpdateUserResponseModel>> UpdateUserAsync(int id, UpdateUserRequestModel requestModel)
	{
		try
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
			if (user is null)
			{
				return Result<UpdateUserResponseModel>.NotFoundError("User with id " + id + " not found.");
			}

			user.Name = requestModel.Name;
			user.Email = requestModel.Email;
			user.PhoneNumber = requestModel.PhoneNumber;
			user.Address = requestModel.Address;
			user.EmergencyType = requestModel.EmergencyType;
			user.EmergencyDetails = requestModel.EmergencyDetails;
			user.TownshipCode = requestModel.TownshipCode;
			user.Role = requestModel.Role;
			user.UserStatus = requestModel.UserStatus;

			_context.Entry(user).State = EntityState.Modified;
			var result = await _context.SaveChangesAsync();

			var model = new UpdateUserResponseModel
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

			return Result<UpdateUserResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			var message = "An error occurred while updating the user for id " + id + ": " + ex.ToString();
			_logger.LogError(message);
			return Result<UpdateUserResponseModel>.SystemError("Internal server error");
		}
	}
}
