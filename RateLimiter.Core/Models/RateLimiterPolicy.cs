
namespace RateLimiter.Core.Models
{
    public class RateLimitPolicy
    {
        public int Limit { get; }
        public TimeSpan Window { get; }

        public RateLimitPolicy(int limit, TimeSpan window)
        {
            Limit = limit;
            Window = window;
        }
    }
}