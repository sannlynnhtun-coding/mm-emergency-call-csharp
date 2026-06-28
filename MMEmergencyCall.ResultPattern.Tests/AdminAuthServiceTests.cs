using MMEmergencyCall.Databases.AppDbContextModels;
using MMEmergencyCall.Shared;
using AdminRegisterRequestModel = MMEmergencyCall.Domain.Admin.Features.Register.AdminRegisterRequestModel;
using AdminRegisterService = MMEmergencyCall.Domain.Admin.Features.Register.AdminRegisterService;
using AdminSignoutService = MMEmergencyCall.Domain.Admin.Features.Signout.AdminSignoutService;
using AdminSigninRequestModel = MMEmergencyCall.Domain.Admin.Features.SignIn.AdminSigninRequestModel;
using AdminSigninService = MMEmergencyCall.Domain.Admin.Features.SignIn.AdminSigninService;
using RefreshTokenModel = MMEmergencyCall.Domain.Admin.Features.RefreshToken.RefreshTokenModel;
using RefreshTokenService = MMEmergencyCall.Domain.Admin.Features.RefreshToken.RefreshTokenService;
using SignoutRefreshTokenModel = MMEmergencyCall.Domain.Admin.Features.Signout.SignoutRefreshTokenModel;

namespace MMEmergencyCall.ResultPattern.Tests;

public class AdminAuthServiceTests
{
    [Fact]
    public async Task RegisterAdmin_returns_success_and_duplicate()
    {
        using var db = new TestDb();
        var service = new AdminRegisterService(Logger.For<AdminRegisterService>(), db.Db);

        var success = await service.RegisterAdminAsync(AdminRegisterRequest("admin@gmail.com", "099100000"));
        Assert.Equal("admin", ResultAssert.Success(success).Role);

        var duplicate = await service.RegisterAdminAsync(AdminRegisterRequest("admin@gmail.com", "099100000"));
        ResultAssert.Duplicate(duplicate);
    }

    [Fact]
    public async Task Signin_returns_success_and_validation_for_bad_credentials()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.User(role: "admin", email: "admin@gmail.com", password: "secret"));
        var service = new AdminSigninService(Logger.For<User>(), db.Db);

        var success = await service.SigninAsync(new AdminSigninRequestModel { Email = "admin@gmail.com", Password = "secret" });
        Assert.False(string.IsNullOrWhiteSpace(ResultAssert.Success(success).Token));

        var failure = await service.SigninAsync(new AdminSigninRequestModel { Email = "admin@gmail.com", Password = "bad" });
        ResultAssert.Validation(failure);
    }

    [Fact]
    public async Task Signin_returns_validation_for_unusable_admin_account()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.User(role: "admin", email: "deleted-admin@gmail.com", password: "secret", status: "Deleted"));
        var service = new AdminSigninService(Logger.For<User>(), db.Db);

        ResultAssert.Validation(await service.SigninAsync(new AdminSigninRequestModel { Email = "deleted-admin@gmail.com", Password = "secret" }));
    }

    [Fact]
    public async Task RefreshToken_returns_success_and_system_error_for_bad_token()
    {
        using var db = new TestDb();
        var sessionId = Guid.NewGuid();
        await db.SaveAsync(Seed.Session(sessionId));
        var service = new RefreshTokenService(Logger.For<RefreshTokenService>(), db.Db);

        var success = await service.RefreshToken(AdminRefreshToken(sessionId, DateTime.Now.AddMinutes(-1)));
        var refreshed = ResultAssert.Success(success).Token.ToDecrypt().ToObject<RefreshTokenModel>();
        Assert.True(refreshed.SessionExpiredTime > DateTime.Now);

        var failure = await service.RefreshToken("bad-token");
        ResultAssert.SystemError(failure);

        ResultAssert.SystemError(await service.RefreshToken(AdminRefreshToken(Guid.NewGuid())));
    }

    [Fact]
    public async Task Signout_returns_success_and_system_error_for_bad_token()
    {
        using var db = new TestDb();
        var sessionId = Guid.NewGuid();
        await db.SaveAsync(Seed.Session(sessionId));
        var service = new AdminSignoutService(Logger.For<AdminSignoutService>(), db.Db);

        var success = await service.Signout(AdminSignoutToken(sessionId));
        Assert.True(ResultAssert.Success(success));

        var failure = await service.Signout("bad-token");
        ResultAssert.SystemError(failure);
    }

    private static AdminRegisterRequestModel AdminRegisterRequest(string email, string phone) => new()
    {
        Name = "Admin",
        Email = email,
        Password = "secret",
        PhoneNumber = phone,
        Address = "Yangon",
        TownshipCode = "TSH",
        EmergencyType = "Fire",
        EmergencyDetails = "Details"
    };

    private static string AdminRefreshToken(Guid sessionId, DateTime? sessionExpiredTime = null)
    {
        return new RefreshTokenModel
        {
            UserId = 1,
            SessionId = sessionId,
            Name = "Admin",
            Email = "admin@gmail.com",
            Role = "admin",
            SessionExpiredTime = sessionExpiredTime ?? DateTime.Now.AddMinutes(5)
        }.ToJson().ToEncrypt();
    }

    private static string AdminSignoutToken(Guid sessionId)
    {
        return new SignoutRefreshTokenModel
        {
            UserId = 1,
            SessionId = sessionId,
            Name = "Admin",
            Email = "admin@gmail.com",
            Role = "admin",
            SessionExpiredTime = DateTime.Now.AddMinutes(5)
        }.ToJson().ToEncrypt();
    }
}
