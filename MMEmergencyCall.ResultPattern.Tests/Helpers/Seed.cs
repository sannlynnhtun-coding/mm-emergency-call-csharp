using MMEmergencyCall.Databases.AppDbContextModels;
using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests.Helpers;

public static class Seed
{
    public static User User(
        int id = 1,
        string email = "user@gmail.com",
        string password = "pass",
        string role = "Normal User",
        string status = "Activated",
        string verified = "Y",
        string townshipCode = "TSH")
    {
        return new User
        {
            UserId = id,
            Name = $"User {id}",
            Email = email,
            Password = password,
            PhoneNumber = $"099{id:000000}",
            Address = "Yangon",
            TownshipCode = townshipCode,
            EmergencyType = "Fire",
            EmergencyDetails = "Details",
            Role = role,
            UserStatus = status,
            IsVerified = verified,
            Otp = "123456"
        };
    }

    public static EmergencyService Service(
        int id = 1,
        int userId = 2,
        string status = "Approved",
        string townshipCode = "TSH",
        string serviceType = "Fire",
        decimal ltd = 16.8m,
        decimal lng = 96.15m)
    {
        return new EmergencyService
        {
            ServiceId = id,
            UserId = userId,
            ServiceGroup = "Emergency",
            ServiceType = serviceType,
            ServiceName = $"Service {id}",
            PhoneNumber = $"091{id:000000}",
            Location = "Yangon",
            Availability = "Y",
            TownshipCode = townshipCode,
            ServiceStatus = status,
            Ltd = ltd,
            Lng = lng
        };
    }

    public static EmergencyRequest Request(
        int id = 1,
        int userId = 1,
        int serviceId = 1,
        int? providerId = 2,
        string status = "Open",
        string townshipCode = "TSH",
        DateTime? requestTime = null)
    {
        return new EmergencyRequest
        {
            RequestId = id,
            UserId = userId,
            ServiceId = serviceId,
            ProviderId = providerId,
            RequestTime = requestTime ?? DateTime.UtcNow,
            Status = status,
            Notes = "Need help",
            TownshipCode = townshipCode
        };
    }

    public static Session Session(Guid id, int userId = 1)
    {
        return new Session
        {
            SessionId = id,
            UserId = userId,
            ExpireTime = DateTime.Now.AddMinutes(5)
        };
    }

    public static StateRegion StateRegion(int id = 1, string code = "SR")
    {
        return new StateRegion
        {
            StateRegionId = id,
            StateRegionCode = code,
            StateRegionNameEn = $"State {id}",
            StateRegionNameMm = $"State MM {id}"
        };
    }

    public static Township Township(int id = 1, string code = "TSH", string stateRegionCode = "SR")
    {
        return new Township
        {
            TownshipId = id,
            TownshipCode = code,
            TownshipNameEn = $"Township {id}",
            TownshipNameMm = $"Township MM {id}",
            StateRegionCode = stateRegionCode
        };
    }

    public static string Status(EnumEmergencyRequestStatus status) => status.ToString();
    public static string ServiceStatus(EnumServiceStatus status) => status.ToString();
}
