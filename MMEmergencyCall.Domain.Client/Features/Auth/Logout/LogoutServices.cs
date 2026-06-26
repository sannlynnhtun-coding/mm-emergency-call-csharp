using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.Logout;

public class LogoutService
{
    private readonly ILogger<User> _logger;
    private readonly AppDbContext _db;

    public LogoutService(ILogger<User> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<Result<bool>> LogoutAsync(LogoutRequestModel requestModel)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == requestModel.UserId);

        if (user == null)
        {
            return Result<bool>.ValidationError("Invalid user.");
        }

       
        var isTokenValid = requestModel.Token.ToDecrypt().Contains(user.Email);
        if (!isTokenValid)
        {
            return Result<bool>.ValidationError("Invalid token.");
        }

       
        //user.SessionToken = null;
        await _db.SaveChangesAsync();

        _logger.LogInformation($"User {user.Name} logged out successfully.");

        return Result<bool>.Success(true);
    }
}
