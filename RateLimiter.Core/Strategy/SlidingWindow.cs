using System.Collections.Concurrent;
using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy 
{
    public class SlidingWindowStrategy : IRateLimitStrategy
    {
        // Optional to use the Storage interface instead of the ConcurrentDictionary
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _storage = new();

        public async Task<bool> IsRequestAllowedAsync(string id, RateLimitPolicy policy)
        {
            var now = DateTime.UtcNow;
            var windowStart = now - policy.Window;

            var queue = _storage.GetOrAdd(id, _ => new ConcurrentQueue<DateTime>());
            
            while (queue.TryPeek(out var timestamp) && timestamp < windowStart)
            {
                queue.TryDequeue(out _);
            }

            if (queue.Count < policy.Limit)
            {
                queue.Enqueue(now);
                return await Task.FromResult(true);
            }
            
            return await Task.FromResult(false);
        }
    }
}
