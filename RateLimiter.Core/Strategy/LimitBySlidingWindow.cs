
using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy
{
    public class LimitBySlidingWindow : ILimitStrategy
    {
        public bool IsAllowed(DateTime reqTime, List<Policy> policies, Record record)
        {
            foreach (var policy in policies)
            {
                DateTime validTime = reqTime - policy.WindowTime;
                
                record.CleanupExpiredRecords(validTime, policy);

                // Check policiy limit
                // Return false if one counter was not honored
                if (record.GetCurrentLimit(policy) >= policy.Limit)
                {
                    return false;
                }
            }

            return true;
        }

        public TimeSpan GetRequiredDelay(DateTime reqTime, List<Policy> policies, Record record)
        {
             
        }
    }
}
