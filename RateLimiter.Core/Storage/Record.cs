using RateLimiter.Core.Models;
using System.Collections.Concurrent;

namespace RateLimiter.Core.Storage
{
    // Overview
    //      Each caller to the RateLimiter will have an associated record.
    //      The record will store the request time, a queue of accepted request timestamps, 
    //      and a list of counters, where each counter represents a specific policy.
    public class Record
    {
        private readonly ConcurrentQueue<DateTime> _timeStamps;
        private readonly ConcurrentDictionary<int, int> _policyCounters;

        public Record()
        {
            _timeStamps = new ConcurrentQueue<DateTime>();
            _policyCounters = new ConcurrentDictionary<int, int>();
        }

        public void AddTimeStamp(DateTime time, Policy policy)
        {
            _timeStamps.Enqueue(time);

            _policyCounters.AddOrUpdate(policy.UId, 1,
                (_, count) => Math.Min(policy.Limit, count + 1));
        }

        public void CleanupExpiredRecords(DateTime newtime, int Id)
        {
            while (_timeStamps.TryPeek(out var oldest) && oldest < newtime)
            {
                _timeStamps.TryDequeue(out _);

                _policyCounters.AddOrUpdate(Id, 0, (_, count) => Math.Max(0, count - 1));
            }
        }

        public int GetCurrentLimit(int id)
        {
            if (_policyCounters.ContainsKey(id))
            {
                return _policyCounters[id];
            }

            return -1;
        }
    }
}