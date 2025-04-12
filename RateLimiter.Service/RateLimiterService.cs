﻿using System.Collections.Concurrent;

using RateLimiter.Core;
using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> :IRateLimiterService<TArg>
    {
        public List<Policy> _rateLimiterPolicies;

        private readonly ConcurrentDictionary<string, CallerRateLimiter<TArg>> _callerLimiters;
        private ILimitStrategy _strategy;


        public RateLimiterService(List<Policy> rateLimiterPolicies) : this(rateLimiterPolicies, new LimitBySlidingWindow()) {}

        public RateLimiterService(List<Policy> rateLimiterPolicies, ILimitStrategy strategy)
        {
            _callerLimiters = new ConcurrentDictionary<string, CallerRateLimiter<TArg>>();
            _rateLimiterPolicies = new List<Policy>();
            
            _strategy = strategy;
        }

        public async Task Perform(Request<TArg> request)
        {
            var callerLimiter = _callerLimiters.GetOrAdd(
                request.Id,
                id => new CallerRateLimiter<TArg>(request, _strategy)
                );

            var reqTime = DateTime.UtcNow;
            await callerLimiter.ExecuteRequest(reqTime, request, _rateLimiterPolicies);
        }

        public void PrintStatistics()
        {
            foreach (var kvp in _callerLimiters)
            {
                kvp.Value.logs(kvp.Key);
            }
        }
    }
}