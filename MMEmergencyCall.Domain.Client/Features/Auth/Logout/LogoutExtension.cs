namespace MMEmergencyCall.Domain.Client.Features.Logout;

public static class LogoutExtension
{
    public static void AddLogoutService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<LogoutService>();
    }
}
