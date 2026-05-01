using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Shenara.Api.Services;

public class AdminAuthService(IConfiguration configuration)
{
    private const int MaxAttempts = 3;
    private static readonly TimeSpan SessionDuration = TimeSpan.FromHours(8);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly ConcurrentDictionary<string, LoginAttemptState> AttemptStates = new();
    private static readonly ConcurrentDictionary<string, AdminSession> Sessions = new();

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
                return AuthResult.Success(CreateSessionToken(normalizedUsername, remoteAddress));
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

    public bool IsTokenValid(string? token)
    {
        PruneExpiredSessions();

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return Sessions.TryGetValue(token, out var session) && session.ExpiresAtUtc > DateTimeOffset.UtcNow;
    }

    private string CreateSessionToken(string username, string? remoteAddress)
    {
        PruneExpiredSessions();

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        Sessions[token] = new AdminSession(username, remoteAddress, DateTimeOffset.UtcNow.Add(SessionDuration));
        return token;
    }

    private static void PruneExpiredSessions()
    {
        var nowUtc = DateTimeOffset.UtcNow;

        foreach (var entry in Sessions)
        {
            if (entry.Value.ExpiresAtUtc <= nowUtc)
            {
                Sessions.TryRemove(entry.Key, out _);
            }
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

public record AdminSession(string Username, string? RemoteAddress, DateTimeOffset ExpiresAtUtc);

public record AuthResult(bool IsSuccess, bool IsLockedOut, string? Token, int RemainingAttempts, DateTimeOffset? LockedUntilUtc)
{
    public static AuthResult Success(string token) => new(true, false, token, 3, null);
    public static AuthResult Failed(int remainingAttempts) => new(false, false, null, remainingAttempts, null);
    public static AuthResult Locked(DateTimeOffset lockedUntilUtc) => new(false, true, null, 0, lockedUntilUtc);
}
