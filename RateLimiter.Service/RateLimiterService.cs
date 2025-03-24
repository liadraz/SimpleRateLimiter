﻿using System.Collections.Concurrent;

using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Core.Service
{
    public class RateLimiterService<TArg>
    {
        private Func<Task> _callAction;
        private IRateLimitStrategy _strategy;
        private List<RateLimitPolicy> _policies;
        private readonly Dictionary<string, SemaphoreSlim> _semaphores = new();
        private readonly ConcurrentDictionary<string, RateLimitRecord> _records = new();
         private readonly object _lock = new(); 

        public RateLimiterService(Func<Task> callAction, List<RateLimitPolicy> policies, IRateLimitStrategy strategy)
        {
            _callAction = callAction;
            _strategy = strategy;
            _policies = policies;
        }

        public async Task<bool> Perform(string callerID)
        {
            SemaphoreSlim semaphore;

            lock (_lock)
            {
                if (!_records.ContainsKey(callerID))
                {
                    _records[callerID] = new RateLimitRecord(_policies);
                    _semaphores[callerID] = new SemaphoreSlim(1, 1); // One concurrent request per client
                }

                semaphore = _semaphores[callerID];
            }

            await semaphore.WaitAsync();
            try
            {
                if (_strategy.CanMakeRequestAsync(_records[callerID], _policies))
                {
                    await _callAction();
                    return true;
                }

                return false;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void LogRequestStats()
        {
            foreach (var id in _records)
            {
                Console.WriteLine($"Request {id.Key}:");

                for (int i = 0; i < _policies.Count; i++)
                {
                    Console.WriteLine($"Policy {i + 1} - Limit: {_policies[i].Limit}, Requests: {id.Value.Counters[i]}");
                }
            }
        }
    }
}