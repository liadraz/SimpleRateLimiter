using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy
{
    public interface IRateLimitStrategy
    {
         public bool CanMakeRequestAsync(RateLimitRecord rec, List<RateLimitPolicy> policies);
    }
}