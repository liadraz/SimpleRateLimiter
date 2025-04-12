using RateLimiter.Core.Models;
using System.Collections.Concurrent;

namespace RateLimiter.Core.Storage
{
    // Overview
    //      Each caller to the RateLimiter will have an associated record.
    //      The record will store queues of accepted request timestamps for each policy.
    public class Record
    {
        private readonly ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>> _policyTimeStamps;


        public Record()
        {
            _policyTimeStamps = new ConcurrentDictionary<Policy, ConcurrentQueue<DateTime>>();
        }

        public void AddTimeStamp(DateTime time)
        {
            foreach (var policyQ in _policyTimeStamps.Keys)
            {
                _policyTimeStamps[policyQ].Enqueue(time);
            }
        }

        public void CleanupExpiredRecords(DateTime newtime, Policy policy)
        {
            while (_policyTimeStamps[policy].TryPeek(out var oldest) && oldest < newtime)
            {
                _policyTimeStamps[policy].TryDequeue(out _);
            }
        }

        public int GetCurrentLimit(Policy policy)
        {
            if (_policyTimeStamps.ContainsKey(policy))
            {
                return _policyTimeStamps[policy].Count;
            }

            return -1;
        }
    }
}