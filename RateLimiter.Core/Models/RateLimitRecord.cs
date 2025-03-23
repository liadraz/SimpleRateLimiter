using System.Collections.Concurrent;

namespace RateLimiter.Core.Models
{
    public class RateLimitRecord
    {
        private ConcurrentQueue<DateTime> _requestTimestamps = new();
        private List<int> _counters = new();
    }
}