﻿using System.Collections.Concurrent;

using RateLimiter.Core;
using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> :IRateLimiterService<TArg>
    {
        private readonly ConcurrentDictionary<string, CallerRateLimiter<TArg>> _callerLimiters;
        private ILimitStrategy _strategy;


        public RateLimiterService() : this(new LimitBySlidingWindow()) {}

        public RateLimiterService(ILimitStrategy strategy)
        {
            _callerLimiters = new ConcurrentDictionary<string, CallerRateLimiter<TArg>>();

            _strategy = strategy;
        }

        public async Task Perform(RequestPacket<TArg> request)
        {
            var callerLimiter = _callerLimiters.GetOrAdd(
                request.Id,
                id => new CallerRateLimiter<TArg>(request.Policies, request.CallAction, _strategy)
                );

            var reqTime = DateTime.UtcNow;
            await callerLimiter.ExecuteRequest(reqTime, request);
        }
    }
}