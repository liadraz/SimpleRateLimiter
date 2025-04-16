
using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;

namespace RateLimiter.Service.Strategy
{
    public class LimitBySlidingWindow : ILimitStrategy
    {
        public bool IsRequestAllowed(DateTime reqTime, Policy policy, Record record)
        {
            bool ret = true;

            DateTime validTime = reqTime - policy.WindowTime;
            record.CleanupExpiredRecords(validTime, policy);

            if (record.GetRecordAmount(policy) >= policy.Limit)
            {
                ret = false;
            }

            System.Console.WriteLine($"IsRequestAllowed -> {ret} {validTime}");
            return ret;
        }
    }
}