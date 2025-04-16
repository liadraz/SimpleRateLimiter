﻿using System.Collections.Concurrent;
using RateLimiter.Service.Models;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> : IRateLimiterService<TArg>
    {
        private readonly ConcurrentDictionary<string, RateLimiterEvaluator<TArg>> _evaluators = new();

        public async Task Perform(Request<TArg> request)
        {
            var reqTime = DateTime.UtcNow;
            var evaluator = _evaluators.GetOrAdd(
                request.Id,
                _ => new RateLimiterEvaluator<TArg>(request.Policies)
            );

            await evaluator.EvaluateAsync(reqTime, (arg) => request.CallAction(arg), request.Arg);
        }
    }
}