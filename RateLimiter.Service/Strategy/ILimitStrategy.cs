using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;

namespace RateLimiter.Service.Strategy
{
    public interface ILimitStrategy
    {
        public bool IsRequestAllowed(DateTime reqTime, Policy policy, Record record);
    }
}