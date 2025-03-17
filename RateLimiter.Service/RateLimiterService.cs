﻿using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Core.Service
{
    public class RateLimiterService<TArg>
    {
        private Func<TArg, Task> _callAction;
        private IRateLimitStrategy _strategy;
        private List<RateLimitPolicy> _policies;
        private readonly SemaphoreSlim _semaphore;

        public RateLimiterService(Func<TArg, Task> callAction, IRateLimitStrategy strategy, List<RateLimitPolicy> policies)
        {
            _callAction = callAction ?? throw new ArgumentNullException(nameof(callAction));
            _strategy = strategy;
            _policies = policies;

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> Perform(string id, TArg arg)
        {
            await _semaphore.WaitAsync();
            
            try
            {
                foreach (var policy in _policies)
                {
                    if (!await _strategy.IsRequestAllowedAsync(id, policy))
                    {
                        return false;
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
                
            await _callAction(arg);
            return true;
        }
    }
}