﻿﻿﻿using System.Collections.Concurrent;

using RateLimiter.Service.Models;
using RateLimiter.Service.Strategy;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> :IRateLimiterService<TArg>
    {
        // Each clientID will have its own RateLimiter which will handle the requests
        private readonly ConcurrentDictionary<string, CallerRateLimiter<TArg>> _callerLimiters;
        
        private readonly ILimitStrategy _strategy;
        private readonly List<Policy> _rateLimiterPolicies;


        public RateLimiterService(ILimitStrategy? strategy = null)
        {
            _callerLimiters = new ConcurrentDictionary<string, CallerRateLimiter<TArg>>();

            _strategy = strategy ?? new LimitBySlidingWindow();
            _rateLimiterPolicies = Policy.RateLimiterPolicies;
        }

        public async Task Perform(Request<TArg> request)
        {
            var callerLimiter = _callerLimiters.GetOrAdd(
                request.Id,
                id => new CallerRateLimiter<TArg>()
                );

            var reqTime = DateTime.UtcNow;
            await callerLimiter.ExecuteRequest(reqTime, request, _rateLimiterPolicies, _strategy);
        }


        // public void PrintStatistics()
        // {
        //     foreach (var kvp in _callerLimiters)
        //     {
        //         kvp.Value.logs(kvp.Key);
        //     }
        // }
    }
}