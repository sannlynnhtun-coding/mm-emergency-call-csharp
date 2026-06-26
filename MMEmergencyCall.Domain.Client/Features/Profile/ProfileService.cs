using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.Profile;

public class ProfileService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(AppDbContext db, ILogger<ProfileService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<ProfileResponseModel>> GetProfileById(int userId)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user is null)
            {
                return Result<ProfileResponseModel>
                       .NotFoundError("User with Id " + userId + " not found.");
            }

            //if(user.UserStatus != ) // Need to validate so only approve status can see their profile

            var model = new ProfileResponseModel()
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                EmergencyType = user.EmergencyType,
                EmergencyDetails = user.EmergencyDetails,
                Role = user.Role,
                TownshipCode = user.TownshipCode,
                UserStatus = user.UserStatus,
            };

            return Result<ProfileResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting the user with id " + userId + " : " + ex.Message;
            _logger.LogError(message);
            return Result<ProfileResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<bool>> DeactivateProfile(int userId)
    {
        try
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (existingUser is null)
            {
                return Result<bool>
                       .NotFoundError("User with Id " + userId + " not found.");
            }

            //if(user.UserStatus != ) // Need to validate so only approve status can deactivate 

            existingUser.UserStatus = nameof(EnumUserStatus.Deactivated);
            _db.Entry(existingUser).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while deactivating the user with id " + userId + " : " + ex.Message;
            _logger.LogError(message);
            return Result<bool>.SystemError("Internal server error");
        }
    }
}
