﻿using System.Collections.Concurrent;

using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;
using RateLimiter.Service.Interface;

namespace RateLimiter.Core.Service
{
    public class RateLimiterService<TArg> : IRateLimiterService<TArg>
    {
        private Func<TArg, Task> _callAction;
        private IRateLimitStrategy _strategy;
        private List<RateLimitPolicy> _policies;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<ClientRequest, RateLimitRecord> _storage = new();

        public RateLimiterService(Func<TArg, Task> callAction, List<RateLimitPolicy> policies, IRateLimitStrategy strategy)
        {
            _callAction = callAction ?? throw new ArgumentNullException(nameof(callAction));
            _strategy = strategy;
            _policies = policies;
            
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> Perform(TArg arg)
        {
            // Each request has client ID when arrives it will be checked.

            // In case the client ID is new -> 
            //      FALSE add it to the Storage, dictionary of clients
            
            //      TRUE get semaphore for the client ID
                //  Critical Code - Can handle several different clients requests
                // _strategy.IsAllowed(storage[id], _policies)
                //      TRUE execute FUNC
            
            //  return counters of all policies

        }
    }
}