using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Shenara.Api.Services;

public class AdminAuthService(IConfiguration configuration)
{
    private const int MaxAttempts = 3;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly ConcurrentDictionary<string, LoginAttemptState> AttemptStates = new();

    public AuthResult ValidateCredentials(string? username, string? password, string? remoteAddress)
    {
        var normalizedUsername = username?.Trim() ?? string.Empty;
        var stateKey = BuildStateKey(normalizedUsername, remoteAddress);
        var attemptState = AttemptStates.GetOrAdd(stateKey, _ => new LoginAttemptState());

        lock (attemptState)
        {
            if (attemptState.LockedUntilUtc.HasValue && attemptState.LockedUntilUtc > DateTimeOffset.UtcNow)
            {
                return AuthResult.Locked(attemptState.LockedUntilUtc.Value);
            }

            var configuredUsername = configuration["Admin:Username"] ?? "admin";
            var configuredPassword = configuration["Admin:Password"] ?? "admin123";
            var isUsernameMatch = SecureEquals(normalizedUsername, configuredUsername);
            var isPasswordMatch = SecureEquals(password ?? string.Empty, configuredPassword);

            if (isUsernameMatch && isPasswordMatch)
            {
                attemptState.Reset();
                return AuthResult.Success(configuration["Admin:Token"] ?? "dev-admin-token");
            }

            attemptState.FailedAttempts++;
            if (attemptState.FailedAttempts >= MaxAttempts)
            {
                attemptState.LockedUntilUtc = DateTimeOffset.UtcNow.Add(LockoutDuration);
                return AuthResult.Locked(attemptState.LockedUntilUtc.Value);
            }

            return AuthResult.Failed(MaxAttempts - attemptState.FailedAttempts);
        }
    }

    private static string BuildStateKey(string username, string? remoteAddress)
    {
        return $"{username.ToLowerInvariant()}|{remoteAddress ?? "unknown"}";
    }

    private static bool SecureEquals(string left, string right)
    {
        var leftBytes = SHA256.HashData(Encoding.UTF8.GetBytes(left));
        var rightBytes = SHA256.HashData(Encoding.UTF8.GetBytes(right));
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private sealed class LoginAttemptState
    {
        public int FailedAttempts { get; set; }
        public DateTimeOffset? LockedUntilUtc { get; set; }

        public void Reset()
        {
            FailedAttempts = 0;
            LockedUntilUtc = null;
        }
    }
}

public record AuthResult(bool IsSuccess, bool IsLockedOut, string? Token, int RemainingAttempts, DateTimeOffset? LockedUntilUtc)
{
    public static AuthResult Success(string token) => new(true, false, token, 3, null);
    public static AuthResult Failed(int remainingAttempts) => new(false, false, null, remainingAttempts, null);
    public static AuthResult Locked(DateTimeOffset lockedUntilUtc) => new(false, true, null, 0, lockedUntilUtc);
}
