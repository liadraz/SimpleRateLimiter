using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy
{
    public class SlidingWindowStrategy : IRateLimitStrategy
    {
        public bool CanMakeRequestAsync(RateLimitRecord rec, List<RateLimitPolicy> policies)
        {
            foreach (var policy in policies)
            {
                DateTime newTime = rec.TimeRequest - policy.TimeWindow;

                while (rec.TimeStampsQ.TryPeek(out DateTime lastTime) && lastTime < newTime)
                {
                    rec.TimeStampsQ.TryDequeue(out _);
                    rec.DecrementCounter(policy.Id);
                }

                if (rec.GetCounter(policy.Id) >= policy.Limit)
                {
                    return false;
                }
                
                rec.IncrementCounter(policy.Id);
            }
            
            rec.TimeStampsQ.Enqueue(rec.TimeRequest);

            return true;
        }
    }
}
