namespace MMEmergencyCall.Domain.Admin.Features.Register;

public class AdminRegisterService
{
    private readonly ILogger<AdminRegisterService> _logger;
    private readonly AppDbContext _db;

    public AdminRegisterService(ILogger<AdminRegisterService> logger, AppDbContext context)
    {
        _logger = logger;
        _db = context;
    }
    
    public async Task<Result<AdminRegisterResponseModel>> RegisterAdminAsync(AdminRegisterRequestModel request)
    {
        try
        {
            var existUser = await _db.Users.AnyAsync(x => x.Email.ToLower() == request.Email.ToLower() ||
                            x.PhoneNumber.ToLower() == request.PhoneNumber.ToLower());
            if (!existUser)
            {
                var user = new User
                {
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Password = request.Password,
                    Address = request.Address,
                    TownshipCode = request.TownshipCode,
                    EmergencyType = request.EmergencyType,
                    EmergencyDetails = request.EmergencyDetails,
                    UserStatus = EnumUserStatus.Activated.ToString(),
                    IsVerified = EnumVerify.Y.ToString(),
                    Role = "admin"
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                var response = new AdminRegisterResponseModel
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
                    UserStatus = user.UserStatus,
                    IsVerified = user.IsVerified
                };
                return Result<AdminRegisterResponseModel>.Success(response);
            }
            else
            {
                return Result<AdminRegisterResponseModel>.DuplicateRecordError("This user already exists.");
            }
        }
        catch (Exception ex)
        {
            string message = "An error occurred while registering the admin: " + ex.ToString();
            _logger.LogError(message);
            return Result<AdminRegisterResponseModel>.SystemError("Internal server error");
        }
    }
}

