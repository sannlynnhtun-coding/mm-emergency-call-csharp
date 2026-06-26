namespace MMEmergencyCall.Domain.Admin.Features.DeleteUser;

public class DeleteUserService
{
	private readonly ILogger<DeleteUserService> _logger;
	private readonly AppDbContext _context;

	public DeleteUserService(ILogger<DeleteUserService> logger, AppDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<Result<bool>> DeleteUserAsync(int id)
	{
		try
		{
			var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
			if (user is null)
				return Result<bool>.NotFoundError("User with id " + id + " not found.");

			user.UserStatus = EnumUserStatus.Deleted.ToString();
			_context.Entry(user).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return Result<bool>.Success(true, "User is deleted successfully");
		}
		catch (Exception ex)
		{
			var message = "An error occurred while deleting the user for id " + id + ": " + ex.ToString();
			_logger.LogError(message);
			return Result<bool>.SystemError("Internal server error");
		}
	}
}
