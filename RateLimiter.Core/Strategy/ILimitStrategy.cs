using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

namespace RateLimiter.Core.Strategy
{
    public interface ILimitStrategy
    {
        public bool IsAllowed(DateTime reqTime, List<Policy> policies, Record record);
    }
}