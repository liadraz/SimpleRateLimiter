using System.Collections.Concurrent;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Storage
{
    // Overview
    //      Each caller to the RateLimiter will have an associated record.
    //      The record will store the request time, a queue of accepted request timestamps, 
    //      and a list of counters, where each counter represents a specific policy.
    public class RateLimitRecord
    {
        public ConcurrentQueue<DateTime> TimeStampsQ { get; } = new();
        public DateTime TimeRequest { get; }
        public Dictionary<int, int> Counters { get; } = new();

        public RateLimitRecord(List<RateLimitPolicy> policies)
        {
            foreach (var policy in policies)
            {
                Counters[policy.Id] = 0;
            }
        }

        public int GetCounter(int policyId)
        {
            if (Counters.ContainsKey(policyId))
            {
                return Counters[policyId];
            }
            return -1;
        }

        public void IncrementCounter(int policyId)
        {
            if (Counters.ContainsKey(policyId))
            {
                Counters[policyId]++;
            }
        }

        public void DecrementCounter(int policyId)
        {
            if (Counters.ContainsKey(policyId))
            {
                Counters[policyId]--;
            }
        }
    }
}