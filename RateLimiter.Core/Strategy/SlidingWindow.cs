using System.Collections.Concurrent;
using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy 
{
    public class SlidingWindowStrategy : IRateLimitStrategy
    {
        public async Task<bool> IsRequestAllowedAsync(RateLimitRecord req, List<RateLimitPolicy> _policies)
        {
            // iterate throgh all policies
            //      Check if Peek time is smaller than reqTS - policy.window
            //      TRUE    dequeue
            //      --PolicyTimeCounter
            
            //  Check req.Count >= policy.limit
            //      return false;

            // in case all policies passed enqueue reqTS
            // return true
        }
    }
}
