using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shenara.Api.Filters;

public class AdminTokenFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuredToken = configuration["Admin:Token"] ?? "dev-admin-token";
        var providedToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);

        if (!string.Equals(configuredToken, providedToken, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Admin authorization is required." });
        }
    }
}
