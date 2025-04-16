﻿using System.Collections.Concurrent;

using RateLimiter.Service.Models;
using RateLimiter.Service.Strategy;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> :IRateLimiterService<TArg>
    {
        private readonly ConcurrentDictionary<string, CallerRateLimiter<TArg>> _callerLimiters;
        
        private readonly ILimitStrategy _strategy;
        private readonly List<Policy> _rateLimiterPolicies;


        public RateLimiterService(ILimitStrategy? strategy = null)
        {
            _callerLimiters = new ConcurrentDictionary<string, CallerRateLimiter<TArg>>();

            _strategy = strategy ?? new LimitBySlidingWindow();
            _rateLimiterPolicies = Policy.RateLimiterPolicies;      // Defined fixed policies for simplicity
        }

        public async Task Perform(Request<TArg> request)
        {
            var callerLimiter = _callerLimiters.GetOrAdd(
                request.Id,
                id => new CallerRateLimiter<TArg>(_rateLimiterPolicies)
                );

            var reqTime = DateTime.UtcNow;
            await callerLimiter.ExecuteRequest(reqTime, request, _rateLimiterPolicies, _strategy);
        }
    }
}