
using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;

namespace RateLimiter.Service.Strategy
{
    public class LimitBySlidingWindow : ILimitStrategy
    {
        public bool IsRequestAllowed(DateTime reqTime, Policy policy, Record record)
        {
            DateTime validTime = reqTime - policy.WindowTime;
            record.CleanupExpiredRecords(validTime, policy);

            if (record.GetAmount(policy) >= policy.Limit)
            {
                return false;
            }

            return true;
        }
    }
}
