
namespace RateLimiter.Service.Models
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

        public override int GetHashCode() => HashCode.Combine(UId);
        public override bool Equals(object? obj) => obj is Policy p && p.UId == UId;
    }
}