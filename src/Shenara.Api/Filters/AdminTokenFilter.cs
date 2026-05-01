using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shenara.Api.Services;

namespace Shenara.Api.Filters;

public class AdminTokenFilter(AdminAuthService authService) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var providedToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);

        if (!authService.IsTokenValid(providedToken))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Admin authorization is required." });
        }
    }
}
