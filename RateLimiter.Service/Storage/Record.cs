
using System.Collections.Concurrent;
using RateLimiter.Service.Models;

namespace RateLimiter.Service.Storage
{
    public class Record
    {
        private readonly ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>> _policyTimeStamps;

        public Record(List<Policy> policies)
        {
            _policyTimeStamps = new ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>>(
                policies.ToDictionary(p => p, _ => new ConcurrentQueue<DateTime>())
            );
        }

        public void Add(DateTime time)
        {
            foreach (var queue in _policyTimeStamps.Values)
            {
                queue.Enqueue(time);
            }
        }

        public int GetCount(Policy policy)
        {
            if (!_policyTimeStamps.TryGetValue(policy, out var queue))
            {
                return 0;
            }

            return queue.Count;
        }

        public DateTime? GetFirstValidTimestamp(Policy policy)
        {
            if (_policyTimeStamps.TryGetValue(policy, out var queue) && queue.TryPeek(out var first))
            {
                return first;
            }

            return null;
        }
        
        public void CleanupExpired(DateTime time, Policy policy)
        {
            var windowStart = time - policy.WindowTime;

            if (_policyTimeStamps.TryGetValue(policy, out var queue))
            {
                while (queue.TryPeek(out var oldest) && oldest < windowStart)
                {
                    _policyTimeStamps[policy].TryDequeue(out _);
                }
            }
        }

    }
}