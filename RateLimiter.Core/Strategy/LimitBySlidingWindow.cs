
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
                
                record.CleanupExpiredRecords(validTime, policy.UId);

                // Check policiy limit
                // Return false if one counter was not honored
                if (record.GetCurrentLimit(policy.UId) >= policy.Limit)
                {
                    return false;
                }
            }

            foreach (var policy in policies)
            {
                // In case request was allowed enqueue the timestamp
                record.AddTimeStamp(reqTime, policy);
            }

            return true;
        }
    }
}
