using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;

// Was not implemented. Follow the Readme file requirements
namespace RateLimiter.Core.Strategy
{
    public class LimitByAbsolute : ILimitStrategy
    {
        public bool IsAllowed(DateTime reqTime, List<Policy> policies, Record record)
        {
            return false;
        }
    }
}