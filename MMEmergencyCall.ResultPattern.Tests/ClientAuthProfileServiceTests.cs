using MMEmergencyCall.Databases.AppDbContextModels;
using MMEmergencyCall.Domain.Client.Features.Logout;
using MMEmergencyCall.Domain.Client.Features.Profile;
using MMEmergencyCall.Domain.Client.Features.Register;
using MMEmergencyCall.Domain.Client.Features.Signin;
using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests;

public class ClientAuthProfileServiceTests
{
    [Fact]
    public async Task Register_and_verify_return_success_and_validation()
    {
        using var db = new TestDb();
        var email = new FakeEmailSender();
        var service = new RegisterService(Logger.For<RegisterService>(), db.Db, email);

        var registered = ResultAssert.Success(await service.RegisterUserAsync(RegisterRequest("new@gmail.com")));
        Assert.Equal("N", registered.IsVerified);
        Assert.Single(email.Sent);

        ResultAssert.Validation(await service.RegisterUserAsync(RegisterRequest("bad@example.com")));
        await db.SaveAsync(Seed.User(2, "existing@gmail.com"));
        ResultAssert.Validation(await service.RegisterUserAsync(RegisterRequest("existing@gmail.com")));

        var verified = ResultAssert.Success(await service.VerifyUserAsync(new VerifyRequestModel
        {
            Email = "new@gmail.com",
            Otp = email.Sent.Single().Otp
        }));
        Assert.Equal("Y", verified.IsVerified);

        ResultAssert.Validation(await service.VerifyUserAsync(new VerifyRequestModel { Email = "new@gmail.com", Otp = "bad" }));
        ResultAssert.Validation(await service.VerifyUserAsync(new VerifyRequestModel { Email = "missing@gmail.com", Otp = "bad" }));
    }

    [Fact]
    public async Task Register_returns_system_error_when_email_sender_fails()
    {
        using var db = new TestDb();
        var service = new RegisterService(Logger.For<RegisterService>(), db.Db, new ThrowingEmailSender());

        ResultAssert.SystemError(await service.RegisterUserAsync(RegisterRequest("mailfail@gmail.com")));
    }

    [Fact]
    public async Task Signin_and_logout_return_success_and_validation()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.User(email: "user@gmail.com", password: "secret"));

        var signin = new SigninService(Logger.For<User>(), db.Db);
        var signedIn = ResultAssert.Success(await signin.SigninAsync(new SigninRequestModel { Email = "user@gmail.com", Password = "secret" }));
        Assert.False(string.IsNullOrWhiteSpace(signedIn.Token));
        ResultAssert.Validation(await signin.SigninAsync(new SigninRequestModel { Email = "user@gmail.com", Password = "bad" }));

        var logout = new LogoutService(Logger.For<User>(), db.Db);
        Assert.True(ResultAssert.Success(await logout.LogoutAsync(new LogoutRequestModel { UserId = 1, Token = signedIn.Token })));
        ResultAssert.Validation(await logout.LogoutAsync(new LogoutRequestModel { UserId = 999, Token = signedIn.Token }));
        ResultAssert.Validation(await logout.LogoutAsync(new LogoutRequestModel { UserId = 1, Token = ClientToken("other@gmail.com") }));
    }

    [Fact]
    public async Task Profile_get_and_deactivate_return_success_and_not_found()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.User());
        var service = new ProfileService(db.Db, Logger.For<ProfileService>());

        Assert.Equal(1, ResultAssert.Success(await service.GetProfileById(1)).UserId);
        ResultAssert.NotFound(await service.GetProfileById(999));

        Assert.True(ResultAssert.Success(await service.DeactivateProfile(1)));
        ResultAssert.NotFound(await service.DeactivateProfile(999));
    }

    private static RegisterRequestModel RegisterRequest(string email) => new()
    {
        Name = "User",
        Email = email,
        PhoneNumber = "099999999",
        Password = "secret",
        Address = "Yangon",
        TownshipCode = "TSH",
        EmergencyType = "Fire",
        EmergencyDetails = "Details"
    };

    private sealed class FakeEmailSender : IEmailSender
    {
        public List<(string Email, string Otp)> Sent { get; } = new();

        public void SendOtp(string toEmail, string otp) => Sent.Add((toEmail, otp));
    }

    private sealed class ThrowingEmailSender : IEmailSender
    {
        public void SendOtp(string toEmail, string otp) => throw new InvalidOperationException("SMTP failed");
    }

    private static string ClientToken(string email)
    {
        return new SigninModel
        {
            UserId = 1,
            Name = "User",
            Email = email,
            SessionExpiredTime = DateTime.Now.AddMinutes(5)
        }.ToJson().ToEncrypt();
    }
}
