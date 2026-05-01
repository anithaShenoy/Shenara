using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shenara.Api.Services;

namespace Shenara.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AdminAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("admin-login")]
    public IActionResult Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username and password are required." });
        }

        var authResult = authService.ValidateCredentials(request.Username, request.Password, HttpContext.Connection.RemoteIpAddress?.ToString());

        if (authResult.IsLockedOut)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new
            {
                message = "Too many failed login attempts. Try again later.",
                lockedUntilUtc = authResult.LockedUntilUtc
            });
        }

        if (!authResult.IsSuccess)
        {
            return Unauthorized(new
            {
                message = "Invalid admin credentials.",
                remainingAttempts = authResult.RemainingAttempts
            });
        }

        return Ok(new { token = authResult.Token, displayName = "Shenara Admin" });
    }
}

public record LoginRequest(string Username, string Password);
