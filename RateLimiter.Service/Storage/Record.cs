
using System.Collections.Concurrent;
using RateLimiter.Service.Models;

namespace RateLimiter.Service.Storage
{
    public class Record
    {
        private readonly ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>> _policyTimeStamps;

        public Record()
        {
            _policyTimeStamps = new ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>>();
        }

        public void AddRecord(DateTime time)
        {
            foreach (var policy in _policyTimeStamps.Keys)
            {
                _policyTimeStamps[policy].Enqueue(time);
            }
        }

        public void CleanupExpiredRecords(DateTime newtime, Policy policy)
        {
            if (_policyTimeStamps.TryGetValue(policy, out var queue))
            {
                while (queue.TryPeek(out var oldest) && oldest < newtime)
                {
                    _policyTimeStamps[policy].TryDequeue(out _);
                }
            }
        }

        public DateTime? GetLastTime(Policy policy)
        {
            return _policyTimeStamps.TryGetValue(policy, out var queue) && queue.TryPeek(out var lastTime) ? lastTime : null;
        }

        public int GetRecordAmount(Policy policy)
        {
            return _policyTimeStamps.TryGetValue(policy, out var queue) ? queue.Count : -1;
        }

    }
}