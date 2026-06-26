namespace MMEmergencyCall.Domain.Admin.Features.SignIn;

public static class AdminSignInServiceExtension
{
    public static void AddAdminSignInService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AdminSigninService>();
    }
}
