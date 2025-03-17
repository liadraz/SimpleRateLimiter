using RateLimiter.Core.Models;

namespace RateLimiter.Core.Strategy
{
    // Interface for rate-limiting strategies
    public interface IRateLimitStrategy
    {
        Task<bool> IsRequestAllowedAsync(string id, RateLimitPolicy policy);
    }
}