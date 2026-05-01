using System.Collections.Concurrent;

namespace Shenara.Api.Services;

public class InquiryProtectionService
{
    private static readonly ConcurrentDictionary<string, AttemptState> AttemptStates = new();
    private static readonly TimeSpan AttemptWindow = TimeSpan.FromDays(1);
    private const int MaxAttemptsPerIpPerDay = 3;

    public InquiryProtectionResult Evaluate(string? remoteAddress, string fingerprint, DateTimeOffset nowUtc)
    {
        var ipKey = string.IsNullOrWhiteSpace(remoteAddress) ? "unknown" : remoteAddress.Trim();
        var attemptState = AttemptStates.GetOrAdd(ipKey, _ => new AttemptState());

        lock (attemptState)
        {
            if (attemptState.WindowStartedUtc == DateTimeOffset.MinValue ||
                nowUtc - attemptState.WindowStartedUtc >= AttemptWindow)
            {
                attemptState.WindowStartedUtc = nowUtc;
                attemptState.TotalAttempts = 0;
                attemptState.Fingerprints.Clear();
            }

            attemptState.TotalAttempts++;

            if (!attemptState.Fingerprints.TryGetValue(fingerprint, out var fingerprintCount))
            {
                attemptState.Fingerprints[fingerprint] = 1;
            }
            else
            {
                attemptState.Fingerprints[fingerprint] = fingerprintCount + 1;
            }

            if (attemptState.TotalAttempts > MaxAttemptsPerIpPerDay)
            {
                return InquiryProtectionResult.Blocked(
                    "Too many inquiry attempts were submitted from this IP address today. Please try again tomorrow.");
            }

            if (attemptState.Fingerprints[fingerprint] > 1)
            {
                return InquiryProtectionResult.Blocked(
                    "This inquiry appears to match a request already submitted today. Please wait for Shenara Event Decor to respond.");
            }

            return InquiryProtectionResult.Allowed();
        }
    }

    private sealed class AttemptState
    {
        public DateTimeOffset WindowStartedUtc { get; set; }
        public int TotalAttempts { get; set; }
        public Dictionary<string, int> Fingerprints { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}

public record InquiryProtectionResult(bool IsAllowed, string? Message)
{
    public static InquiryProtectionResult Allowed() => new(true, null);
    public static InquiryProtectionResult Blocked(string message) => new(false, message);
}
