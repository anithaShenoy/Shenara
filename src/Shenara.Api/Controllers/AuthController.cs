using Microsoft.AspNetCore.Mvc;

namespace Shenara.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var username = configuration["Admin:Username"] ?? "admin";
        var password = configuration["Admin:Password"] ?? "admin123";
        var token = configuration["Admin:Token"] ?? "dev-admin-token";

        if (!string.Equals(request.Username, username, StringComparison.OrdinalIgnoreCase) || request.Password != password)
        {
            return Unauthorized(new { message = "Invalid admin credentials." });
        }

        return Ok(new { token, displayName = "Shenara Admin" });
    }
}

public record LoginRequest(string Username, string Password);
