namespace MMEmergencyCall.Domain.Admin.Middlewares;

public class AdminAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetService<AppDbContext>();

        if (dbContext == null)
        {
            context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Something went wrong."));
            return;
        }

        if (!context.HttpContext!.Request.Headers["Token"].Any())
        {
            context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Token is missing."));
            return;
        }

        try
        {
            var token = context.HttpContext.Request.Headers["Token"].ToString();
            var item = token.ToDecrypt().ToObject<AdminSignInModel>();

            if (!await IsExistAdmin(dbContext, item.UserId))
            {
                context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Admin does not exist."));
                return;
            }

            if (item.SessionExpiredTime <= DateTime.Now)
            {
                context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Session has expired."));
                return;
            }

            context.HttpContext.Items["UserId"] = item.UserId;
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Invalid token."));
            return;
        }
    }

    private static async Task<bool> IsExistAdmin(AppDbContext dbContext, int userId)
    {
        return await dbContext.Users.AnyAsync(u =>
            u.UserId == userId &&
            u.Role.ToLower() == "admin" &&
            u.IsVerified == EnumVerify.Y.ToString() &&
            u.UserStatus != EnumUserStatus.Deleted.ToString());
    }
}
