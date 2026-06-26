namespace MMEmergencyCall.Domain.Client.Middlewares;

public class UserAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
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

        if (!context.HttpContext.Request.Headers["Token"].Any())
        {
            context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("Token is missing."));
            return;
        }

        try
        {
            var token = context.HttpContext.Request.Headers["Token"].ToString();
            var item = token.ToDecrypt().ToObject<SigninModel>();

            if (!await IsUserExist(dbContext, item.UserId))
            {
                context.Result = new UnauthorizedObjectResult(Result<object?>.UnauthorizedError("User does not exist."));
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
        }
    }

    private async Task<bool> IsUserExist(AppDbContext dbContext, int userId)
    {
        return await dbContext.Users.AnyAsync(u => u.UserId == userId && u.Role == "Normal User");
    }
}
