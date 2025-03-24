
namespace RateLimiter.Core.Models
{
    public class RateLimitPolicy
    {
        private static int _idCounter = 0;

        public int Id { get; }
        public int Limit { get; }
        public TimeSpan TimeWindow { get; }

        public RateLimitPolicy(int limit, TimeSpan timeWindow)
        {
            Id = ++_idCounter;

            Limit = limit;
            TimeWindow = timeWindow;
        }
    }
}