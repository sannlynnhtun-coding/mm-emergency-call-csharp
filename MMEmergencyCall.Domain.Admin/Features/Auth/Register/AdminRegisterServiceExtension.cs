namespace MMEmergencyCall.Domain.Admin.Features.Register;

public static class AdminRegisterServiceExtension
{
    public static void AddAdminRegisterService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AdminRegisterService>();
    }
}