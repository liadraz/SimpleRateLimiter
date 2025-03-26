
namespace RateLimiter.Core.Models
{
    public class Policy
    {
        private static int _idCounter = 0;

        public int UId { get; }
        public int Limit { get; }
        public TimeSpan WindowTime { get; }

        public Policy(int limit, TimeSpan windowTime)
        {
            UId = ++_idCounter;

            Limit = limit;
            WindowTime = windowTime;
        }
    }
}